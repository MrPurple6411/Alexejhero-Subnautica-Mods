using AlexejheroYTB.Common;
using Harmony;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Logger = AlexejheroYTB.Common.Logger;

namespace AlexejheroYTB.PickupFullCarryalls
{
    public static class QMod
    {
        public static string assembly = Assembly.GetExecutingAssembly().GetName().Name;

        public static void Patch()
        {
            try
            {
                HarmonyHelper.Patch();

                Logger.Log("Patched successfully!");

                PFC_Config.Enable = PlayerPrefsExtra.GetBool("pfcEnable", true);

                Logger.Log("Obtained values from config");

                OptionsPanelHandler.RegisterModOptions(new Options("Pickup Full Carry-alls"));

                Logger.Log("Registered mod options");

                /*
                ItemActionHandler.RegisterMiddleClickAction(TechType.LuggageBag, InventoryOpener.OnMiddleClick, "open storage");
                ItemActionHandler.RegisterMiddleClickAction(TechType.SmallStorage, InventoryOpener.OnMiddleClick, "open storage");

                Logger.Log("Registered middle click actions");
                */
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Initializing);
            }
        }
    }

    public static class InventoryOpener
    {
        public static InventoryItem LastOpened;
        public static uGUI_ItemsContainer InventoryUGUI;
        public static bool DontEnable;

        public static void OnMiddleClick(InventoryItem item)
        {
            try
            {
                if (!PFC_Config.Enable)
                {
                    ErrorMessage.AddMessage($"[{QMod.assembly}] Mod is disabled!");
                    return;
                }

                Vector2int cursorPosition = GetCursorPosition();

                DontEnable = true;
                Player.main.GetPDA().Close();
                DontEnable = false;

                StorageContainer container = item.item.gameObject.GetComponentInChildren<PickupableStorage>().storageContainer;
                container.Open();
                container.onUse.Invoke();

                if (Inventory.main.container.GetItems(item.item.GetTechType()).Contains(item))
                {
                    if (LastOpened != null)
                    {
                        LastOpened.isEnabled = true;
                        GetIconForItem(LastOpened)?.SetChroma(1f);
                    }
                    item.isEnabled = false;
                    GetIconForItem(item)?.SetChroma(0f);
                    LastOpened = item;
                }

                GameObject.FindObjectOfType<GameInput>().StartCoroutine(ResetCursor(cursorPosition));
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }

        public static uGUI_ItemIcon GetIconForItem(InventoryItem item)
        {
            try
            {
                Dictionary<InventoryItem, uGUI_ItemIcon> items = typeof(uGUI_ItemsContainer).GetField("items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(InventoryUGUI) as Dictionary<InventoryItem, uGUI_ItemIcon>;
                return items[item];
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.InPatch, QMod.assembly);
                return null;
            }
        }

        #region Mouse Position

        public static IEnumerator ResetCursor(Vector2int position)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            SetCursorPosition(position);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
        }

        public static Vector2int GetCursorPosition()
        {
            GetCursorPos(out Point point);
            return new Vector2int(point.X, point.Y);
        }
        public static void SetCursorPosition(Vector2int position)
        {
            SetCursorPos(position.x, position.y);
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point pos);
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        #endregion
    }

    public static class Patches
    {
        #region Storage Pickup

        [HarmonyPatch(typeof(PickupableStorage), "OnHandClick")]
        public static class PickupableStorage_OnHandClick
        {
            [HarmonyPrefix]
            public static bool Prefix(PickupableStorage __instance, GUIHand hand)
            {
                try
                {
                    TechType type = __instance.pickupable.GetTechType();
                    if (PFC_Config.Enable && type == TechType.LuggageBag || type == TechType.SmallStorage)
                    {
                        __instance.pickupable.OnHandClick(hand);
                        Logger.Log("Picked up a carry-all", QMod.assembly);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception(e, LoggedWhen.InPatch, QMod.assembly);
                    return false;
                }
            }
        }

        [HarmonyPatch(typeof(PickupableStorage), "OnHandHover")]
        public static class PickupableStorage_OnHandHover
        {
            [HarmonyPrefix]
            public static bool Prefix(PickupableStorage __instance, GUIHand hand)
            {
                try
                {
                    TechType type = __instance.pickupable.GetTechType();
                    if (PFC_Config.Enable && type == TechType.LuggageBag || type == TechType.SmallStorage)
                    {
                        __instance.pickupable.OnHandHover(hand);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception(e, LoggedWhen.InPatch, QMod.assembly);
                    return false;
                }
            }
        }

        #endregion

        #region Destruction Prevention

        [HarmonyPatch(typeof(ItemsContainer), "IItemsContainer.AllowedToRemove")]
        public static class IItemsContainer_AllowedToRemove
        {
            [HarmonyPrefix]
            public static bool Prefix(ItemsContainer __instance, bool __result, Pickupable pickupable, bool verbose)
            {
                try
                {
                    if (!PFC_Config.Enable) return true;
                    if (__instance != Inventory.main.container) return true;
                    if (pickupable == InventoryOpener.LastOpened?.item)
                    {
                        __result = false;
                        return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Exception(e, LoggedWhen.InPatch, QMod.assembly);
                    return false;
                }
            }
        }

        [HarmonyPatch(typeof(uGUI_ItemsContainer), "Init")]
        public static class uGUI_ItemsContainer_Init
        {
            [HarmonyPostfix]
            public static void Postfix(uGUI_ItemsContainer __instance, ItemsContainer container)
            {
                try
                {
                    if (container == Inventory.main.container)
                    {
                        InventoryOpener.InventoryUGUI = __instance;
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception(e, LoggedWhen.InPatch, QMod.assembly);
                }
            }
        }

        [HarmonyPatch(typeof(PDA), "Close")]
        public static class PDA_Close
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                try
                {
                    if (InventoryOpener.LastOpened != null && !InventoryOpener.DontEnable)
                    {
                        InventoryOpener.LastOpened.isEnabled = true;
                        InventoryOpener.GetIconForItem(InventoryOpener.LastOpened)?.SetChroma(1f);
                        InventoryOpener.LastOpened = null;
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception(e, LoggedWhen.InPatch, QMod.assembly);
                    return;
                }
            }
        }

        #endregion

        #region Middle Click Handling

        [HarmonyPatch(typeof(uGUI_InventoryTab), "OnPointerClick")]
        public static class uGUI_InventoryTab_OnPointerClick
        {
            [HarmonyPrefix]
            public static bool Prefix(InventoryItem item, int button)
            {
                if (ItemDragManager.isDragging)
                {
                    return true;
                }
                if (button == 2)
                {
                    Inventory.main.GetInstanceMethod("ExecuteItemAction").Invoke(Inventory.main, new object[] { (ItemAction)2019, item });
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
                TechType itemTechType = item.item.GetTechType();
                if (action != (ItemAction)2019) return true;
                if (itemTechType != TechType.LuggageBag && itemTechType != TechType.SmallStorage) return true;

                InventoryOpener.OnMiddleClick(item);

                return false;
            }
        }

        [HarmonyPatch(typeof(TooltipFactory), "ItemActions")]
        public static class TooltipFactory_ItemActions
        {
            [HarmonyPostfix]
            public static void Postfix(StringBuilder sb, InventoryItem item)
            {
                TechType itemTechType = item.item.GetTechType();
                if (itemTechType == TechType.LuggageBag || itemTechType == TechType.SmallStorage)
                {
                    sb.Append("\n");
                    typeof(TooltipFactory).GetMethod("WriteAction", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { sb, "MMB", "open storage" });
                }
            }
        }

        #endregion
    }

    public static class PFC_Config
    {
        public static bool Enable = true;
    }

    public class Options : ModOptions
    {
        public Options(string name) : base(name)
        {
            try
            {
                ToggleChanged += OnToggleChanged;
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Options);
            }
        }

        public override void BuildModOptions()
        {
            try
            {
                AddToggleOption("pfcEnable", "Enable", PFC_Config.Enable);
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Options);
            }
        }

        public void OnToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            try
            {
                if (e.Id == "pfcEnable")
                {
                    if (e.Value) Logger.Log("Enabled mod", QMod.assembly);
                    else Logger.Log("Disabled mod", QMod.assembly);
                    PFC_Config.Enable = e.Value;
                    PlayerPrefsExtra.SetBool("pfcEnable", e.Value);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, LoggedWhen.Options);
            }
        }
    }
}