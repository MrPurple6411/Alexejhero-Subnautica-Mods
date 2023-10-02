namespace InstantBulkheadAnimations;
using HarmonyLib;

internal static class Patches
{

    [HarmonyPatch(typeof(BulkheadDoor), nameof(BulkheadDoor.OnHandClick))]
    [HarmonyPrefix]
    public static bool BulkheadDoor_OnHandClick_Prefix(BulkheadDoor __instance, GUIHand hand)
    {
        if (__instance.enabled && PlayerCinematicController.cinematicModeCount <= 0 && Options.Enable)
        {
            __instance.SetState(!__instance.opened);
            Plugin.Logger.LogDebug("Bulkhead animation skipped!");
            return false;
        }
        else
        {
            return true;
        }
    }
}