namespace NoMenuPause;
using HarmonyLib;
using UWE;

internal static class Patches
{

    [HarmonyPatch(typeof(FreezeTime), nameof(FreezeTime.Begin))]
    [HarmonyPrefix]
    public static bool FreezeTime_Begin_Prefix(FreezeTime.Id id)
    {
        return id != FreezeTime.Id.IngameMenu || !Plugin.NMP;
    }
}