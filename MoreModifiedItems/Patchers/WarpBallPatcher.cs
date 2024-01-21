namespace MoreModifiedItems.Patchers;

using HarmonyLib;
using MoreModifiedItems.WarpStabilizationSuit;
using UnityEngine;

[HarmonyPatch(typeof(WarpBall))]
internal static class WarpBallPatcher
{
    [HarmonyPatch(nameof(WarpBall.Warp))]
    [HarmonyPrefix]
    public static bool WarpBall_Warp_Prefix(WarpBall __instance, GameObject target)
    {
        if(!target.TryGetComponent(out Player player))
        {
            return true;
        }

        if (!TechTypeExtensions.FromString("WarpStabilizationGloves", out TechType warpStabilizationGloves, true))
        {
            // If we can't get the TechType of the gloves, then something is wrong.
            Plugin.Log.LogError("Failed to get TechType of WarpStabilizationGloves.... This really should never happen.... please report");
            return true;
        }

        if (Inventory.main.equipment.GetTechTypeInSlot("Gloves") != warpStabilizationGloves)
        {
            // If the player is not wearing the gloves, then we want to allow the warp to happen.
            return true;
        }

        InventoryItem suit = Inventory.main.equipment.GetItemInSlot("Body");
        if (suit == null)
        {
            // If the player is not wearing a suit, then we want to allow the warp to happen.
            return true;
        }
        
        if (!suit.item.TryGetComponent(out AntiWarperBehaviour _))
        {
            // If the player is not wearing the suit, then we want to allow the warp to happen.
            return true;
        }

        return false;
    }
}
