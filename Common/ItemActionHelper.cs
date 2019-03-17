using Harmony;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AlexejheroYTB.Common
{
    public enum MouseButton
    {
        Left = 0,
        Middle = 2,
    }

    public class ItemActionHelper
    {
        public TechType TargetTechType;
        public Action<InventoryItem> Callback;
        public string Tooltip;
        public Predicate<InventoryItem> Condition;
        public MouseButton Button;

        public const ItemAction LeftClickItemAction = (ItemAction)2020;
        public const ItemAction MiddleClickItemAction = (ItemAction)2019;

        public static readonly Dictionary<TechType, ItemActionHelper> RegisteredLMBActions = new Dictionary<TechType, ItemActionHelper>();
        public static readonly Dictionary<TechType, ItemActionHelper> RegisteredMMBActions = new Dictionary<TechType, ItemActionHelper>();

        public static ItemActionHelper RegisterAction(MouseButton button, TechType targetTechType, Action<InventoryItem> callback, string tooltip, Predicate<InventoryItem> condition)
        {
            ItemActionHelper action = new ItemActionHelper()
            {
                TargetTechType = targetTechType,
                Callback = callback,
                Tooltip = tooltip,
                Condition = condition,
                Button = button,
            };

            if (button == MouseButton.Left) RegisteredLMBActions.Add(targetTechType, action);
            else if (button == MouseButton.Middle) RegisteredMMBActions.Add(targetTechType, action);

            return action;
        }

        public static class Patches
        {
            [HarmonyPatch(typeof(uGUI_InventoryTab), "OnPointerClick")]
#pragma warning disable IDE1006 // Naming Styles
            public static class uGUI_InventoryTab_OnPointerClick
#pragma warning restore IDE1006 // Naming Styles
            {
                [HarmonyPrefix]
                public static bool Prefix(InventoryItem item, int button)
                {
                    if (ItemDragManager.isDragging) return true;

                    bool hasLMBaction = RegisteredLMBActions.TryGetValue(item.item.GetTechType(), out ItemActionHelper LMBaction);
                    bool hasMMBaction = RegisteredMMBActions.TryGetValue(item.item.GetTechType(), out ItemActionHelper MMBaction);
                    if (!hasLMBaction && !hasMMBaction) return true;

                    if (hasLMBaction && button == (int)MouseButton.Left && (LMBaction?.Condition(item)).ToNormalBool())
                    {
                        Inventory.main.GetInstanceMethod("ExecuteItemAction").Invoke(Inventory.main, new object[] { LeftClickItemAction, item });
                        return false;
                    }
                    if (hasMMBaction && button == (int)MouseButton.Middle && (MMBaction?.Condition(item)).ToNormalBool())
                    {
                        Inventory.main.GetInstanceMethod("ExecuteItemAction").Invoke(Inventory.main, new object[] { MiddleClickItemAction, item });
                        return false;
                    }
                    else return true;
                }
            }

            [HarmonyPatch(typeof(Inventory), "ExecuteItemAction")]
            public static class Inventory_ExecuteItemAction
            {
                [HarmonyPrefix]
                public static bool Prefix(ItemAction action, InventoryItem item)
                {
                    bool hasLMBaction = RegisteredLMBActions.TryGetValue(item.item.GetTechType(), out ItemActionHelper LMBaction);
                    bool hasMMBaction = RegisteredMMBActions.TryGetValue(item.item.GetTechType(), out ItemActionHelper MMBaction);
                    if (!hasLMBaction && !hasMMBaction) return true;

                    if (hasLMBaction && action == LeftClickItemAction && (LMBaction?.Condition(item)).ToNormalBool())
                    {
                        LMBaction.Callback(item);
                        return false;
                    }
                    if (hasMMBaction && action == MiddleClickItemAction && (MMBaction?.Condition(item)).ToNormalBool())
                    {
                        MMBaction.Callback(item);
                        return false;
                    }
                    else return true;
                }
            }

            [HarmonyPatch(typeof(TooltipFactory), "ItemActions")]
            public static class TooltipFactory_ItemActions
            {
                [HarmonyPostfix]
                public static void Postfix(StringBuilder sb, InventoryItem item)
                {
                    bool hasLMBaction = RegisteredLMBActions.TryGetValue(item.item.GetTechType(), out ItemActionHelper LMBaction);
                    bool hasMMBaction = RegisteredMMBActions.TryGetValue(item.item.GetTechType(), out ItemActionHelper MMBaction);
                    if (hasLMBaction || hasMMBaction) sb.Append("\n");

                    if (hasLMBaction && (LMBaction?.Condition(item)).ToNormalBool())
                    {
                        string mouseLeft = typeof(TooltipFactory).GetField("stringLeftHand", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as string;

                        typeof(TooltipFactory).GetMethod("WriteAction", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { sb, mouseLeft, LMBaction.Tooltip });
                    }
                    if (hasMMBaction && (MMBaction?.Condition(item)).ToNormalBool())
                    {
                        string mouseMiddle = "<color=#ADF8FFFF></color>";

                        typeof(TooltipFactory).GetMethod("WriteAction", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { sb, mouseMiddle, MMBaction.Tooltip });
                    }
                }
            }
        }
    }
}
