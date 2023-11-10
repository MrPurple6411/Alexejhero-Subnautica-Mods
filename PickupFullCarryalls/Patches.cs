namespace PickupableStorageEnhanced;
using HarmonyLib;

[HarmonyPatch]
public static class Patches
{
    #region Storage Pickup

    [HarmonyPatch(typeof(PickupableStorage), nameof(PickupableStorage.OnHandClick))]
    [HarmonyPrefix]
    public static bool PickupableStorage_OnHandClick_Prefix(PickupableStorage __instance, GUIHand hand)
    {
        TechType type = __instance.pickupable.GetTechType();
        if (PFC_Config.Enable && (type == TechType.LuggageBag || type == TechType.SmallStorage))
        {
            __instance.pickupable.OnHandClick(hand);
            Plugin.Logger.LogDebug("Picked up a carry-all");
            return false;
        }
        else
        {
            return true;
        }
    }

    [HarmonyPatch(typeof(PickupableStorage), nameof(PickupableStorage.OnHandHover))]
    [HarmonyPrefix]
    public static bool PickupableStorage_OnHandHover_Prefix(PickupableStorage __instance, GUIHand hand)
    {
        TechType type = __instance.pickupable.GetTechType();
        if (PFC_Config.Enable && (type == TechType.LuggageBag || type == TechType.SmallStorage))
        {
            __instance.pickupable.OnHandHover(hand);
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion

    #region Destruction Prevention

    [HarmonyPatch(typeof(ItemsContainer), "IItemsContainer.AllowedToRemove")]
    [HarmonyPrefix]
    public static bool IItemsContainer_AllowedToRemove_Prefix(ItemsContainer __instance, ref bool __result, Pickupable pickupable, bool verbose)
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

    [HarmonyPatch(typeof(uGUI_ItemsContainer), nameof(uGUI_ItemsContainer.Init))]
    [HarmonyPostfix]
    public static void UGUI_ItemsContainer_Init_Postfix(uGUI_ItemsContainer __instance, ItemsContainer container)
    {
        if (container == Inventory.main.container)
        {
            InventoryOpener.InventoryUGUI = __instance;
        }
    }

    [HarmonyPatch(typeof(PDA), nameof(PDA.Close))]
    [HarmonyPrefix]
    public static void PDA_Close_Prefix(PDA __instance)
    {
        if (InventoryOpener.LastOpened != null)
        {
            InventoryOpener.LastOpened.isEnabled = true;
            InventoryOpener.GetIconForItem(InventoryOpener.LastOpened)?.SetChroma(1f);
            InventoryOpener.LastOpened = null;
            InventoryOpener.LastOpenedContainer.OnClosePDA(__instance);
            InventoryOpener.LastOpenedContainer = null;
        }
    }

    #endregion
}
