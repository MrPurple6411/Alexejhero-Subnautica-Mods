namespace ConfigurableDrillableCount;

using HarmonyLib;

[HarmonyPatch]
internal static class Patches
{
    [HarmonyPatch(typeof(Drillable), nameof(Drillable.Start))]
    [HarmonyPostfix]
    public static void Drillable_Start_Postfix(Drillable __instance)
    {
        __instance.gameObject.EnsureComponent<CDC_Config>();
        Plugin.Logger.LogInfo("Added component to Drillable!");
    }
}