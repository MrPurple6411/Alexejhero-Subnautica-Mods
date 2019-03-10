using AlexejheroYTB.Common;
using Harmony;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;
using System;
using System.Reflection;
using System.Text;
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

                ItemActionHandler.RegisterMiddleClickAction(TechType.LuggageBag, Patches.OnMiddleClick, "open storage");
                ItemActionHandler.RegisterMiddleClickAction(TechType.SmallStorage, Patches.OnMiddleClick, "open storage");
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Initializing);
            }
        }
    }

    public static class Patches
    {
        public static void OnMiddleClick(InventoryItem item)
        {
            Player.main.GetPDA().Close();
            StorageContainer container = item.item.gameObject.GetComponentInChildren<PickupableStorage>().storageContainer;
            container.Open();
            container.onUse.Invoke();
        }

        [HarmonyPatch(typeof(PickupableStorage), "OnHandClick")]
        public static class PickupableStorage_OnHandClick
        {
            [HarmonyPrefix]
            public static bool Prefix(PickupableStorage __instance, GUIHand hand)
            {
                try
                {
                    if (PFC_Config.Enable)
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
                    Logger.Exception(e, LoggedWhen.InPatch);
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
                    if (PFC_Config.Enable)
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
                    Logger.Exception(e, LoggedWhen.InPatch);
                    return false;
                }
            }
        }

        /*[HarmonyPatch(typeof(uGUI_InventoryTab), "OnPointerClick")]
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
                    ErrorMessage.AddDebug("Middle clicked an item!");
                    Inventory.main.GetInstanceMethod("ExecuteItemAction").Invoke(Inventory.main, new object[] { (ItemAction)1337, item });
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
                if ((itemTechType == TechType.LuggageBag || itemTechType == TechType.SmallStorage) && action == (ItemAction)1337)
                {
                    ErrorMessage.AddDebug("Executed custom item action for " + item.item.GetTechType());
                    Player.main.GetPDA().Close();
                    StorageContainer container = item.item.gameObject.GetComponentInChildren<PickupableStorage>().storageContainer;
                    container.Open();
                    container.onUse.Invoke();
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
                TechType itemTechType = item.item.GetTechType();
                if (itemTechType == TechType.LuggageBag || itemTechType == TechType.SmallStorage)
                {
                    sb.Append("\n");
                    typeof(TooltipFactory).GetMethod("WriteAction", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { sb, "MMB", "open storage" });
                }
            }
        }*/
    }

    public class PFC_Config
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