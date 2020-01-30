using AlexejheroYTB.Common;
using Harmony;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using Logger = AlexejheroYTB.Common.Logger;

namespace AlexejheroYTB.PickupFullCarryalls
{
    public static class QMod
    {
        public static void Patch()
        {
            HarmonyHelper.Patch();
            Logger.Log("Patched successfully!");

            PFC_Config.Enable = PlayerPrefsExtra.GetBool("pfcEnable", true);
            Logger.Log("Obtained values from config");

            OptionsPanelHandler.RegisterModOptions(new Options("Pickup Full Carry-alls"));
            Logger.Log("Registered mod options");

            ItemActionHandler.RegisterMiddleClickAction(TechType.LuggageBag, InventoryOpener.OnMiddleClick, "open storage", InventoryOpener.Condition);
            ItemActionHandler.RegisterMiddleClickAction(TechType.SmallStorage, InventoryOpener.OnMiddleClick, "open storage", InventoryOpener.Condition);
        }
    }

    public static class InventoryOpener
    {
        public static InventoryItem LastOpened;
        public static uGUI_ItemsContainer InventoryUGUI;
        public static bool DontEnable;

        public static void OnMiddleClick(InventoryItem item)
        {
            Vector2int cursorPosition = GetCursorPosition();

            DontEnable = true;
            Player.main.GetPDA().Close();
            DontEnable = false;

            StorageContainer container = item.item.gameObject.GetComponentInChildren<PickupableStorage>().storageContainer;
            container.Open();
            container.onUse.Invoke();

            if (PlayerInventoryContains(item))
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

        public static bool Condition(InventoryItem item)
        {
            if (!PFC_Config.Enable) return false;
            if (!CanOpen(item)) return false;
            return true;
        }

        public static bool CanOpen(InventoryItem item)
        {
            if (PFC_Config.AllowMMB == "Yes") return true;
            if (PFC_Config.AllowMMB == "No") return false;
            if (PFC_Config.AllowMMB == "Only in player inventory" && PlayerInventoryContains(item)) return true;
            return false;
        }

        public static bool PlayerInventoryContains(InventoryItem item)
        {
            IList<InventoryItem> matchingItems = Inventory.main.container.GetItems(item.item.GetTechType());
            if (matchingItems == null) return false;
            return matchingItems.Contains(item);
        }

        public static uGUI_ItemIcon GetIconForItem(InventoryItem item)
        {
            Dictionary<InventoryItem, uGUI_ItemIcon> items = typeof(uGUI_ItemsContainer).GetField("items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(InventoryUGUI) as Dictionary<InventoryItem, uGUI_ItemIcon>;
            return items[item];
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
                TechType type = __instance.pickupable.GetTechType();
                if (PFC_Config.Enable && type == TechType.LuggageBag || type == TechType.SmallStorage)
                {
                    __instance.pickupable.OnHandClick(hand);
                    Logger.Log("Picked up a carry-all");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(PickupableStorage), "OnHandHover")]
        public static class PickupableStorage_OnHandHover
        {
            [HarmonyPrefix]
            public static bool Prefix(PickupableStorage __instance, GUIHand hand)
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
        }

        #endregion

        #region Destruction Prevention

        [HarmonyPatch(typeof(ItemsContainer), "IItemsContainer.AllowedToRemove")]
        public static class IItemsContainer_AllowedToRemove
        {
            [HarmonyPrefix]
            public static bool Prefix(ItemsContainer __instance, bool __result, Pickupable pickupable, bool verbose)
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
        }

        [HarmonyPatch(typeof(uGUI_ItemsContainer), "Init")]
        public static class uGUI_ItemsContainer_Init
        {
            [HarmonyPostfix]
            public static void Postfix(uGUI_ItemsContainer __instance, ItemsContainer container)
            {
                if (container == Inventory.main.container)
                {
                    InventoryOpener.InventoryUGUI = __instance;
                }
            }
        }

        [HarmonyPatch(typeof(PDA), "Close")]
        public static class PDA_Close
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (InventoryOpener.LastOpened != null && !InventoryOpener.DontEnable)
                {
                    InventoryOpener.LastOpened.isEnabled = true;
                    InventoryOpener.GetIconForItem(InventoryOpener.LastOpened)?.SetChroma(1f);
                    InventoryOpener.LastOpened = null;
                }
            }
        }

        #endregion
    }

    public static class PFC_Config
    {
        public static bool Enable
        {
            get
            {
                return PlayerPrefsExtra.GetBool("pfcEnable", true);
            }
            set
            {
                PlayerPrefsExtra.SetBool("pfcEnable", value);
                PlayerPrefs.Save();
            }
        }

        public static string AllowMMB
        {
            get
            {
                return PlayerPrefs.GetString("pfcMMB", "Yes");
            }
            set
            {
                PlayerPrefs.SetString("pfcMMB", value);
                PlayerPrefs.Save();
            }
        }

        public static string[] AllowMMBOptions =
        {
            "Yes",
            "Only in player inventory",
            "No",
        };
    }

    public class Options : ModOptions
    {
        public Options(string name) : base(name)
        {
            ToggleChanged += OnToggleChanged;
            ChoiceChanged += OnChoiceChanged;
        }

        public override void BuildModOptions()
        {
            AddToggleOption("pfcEnable", "Enable", PFC_Config.Enable);
            AddChoiceOption("pfcMMB", "Open storage in inventory", PFC_Config.AllowMMBOptions, PFC_Config.AllowMMB);
        }

        public void OnToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id == "pfcEnable")
            {
                if (e.Value) Logger.Log("Enabled mod");
                else Logger.Log("Disabled mod");
                PFC_Config.Enable = e.Value;
            }
        }
        public void OnChoiceChanged(object sender, ChoiceChangedEventArgs e)
    {
            if (e.Id == "pfcMMB")
            {
                Logger.Log($"Set storage opening in inventory to: \"{e.Value}\"");
                PFC_Config.AllowMMB = e.Value;
            }
        }
    }
}