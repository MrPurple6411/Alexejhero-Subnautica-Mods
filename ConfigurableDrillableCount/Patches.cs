namespace ConfigurableDrillableCount;

using HarmonyLib;
using UnityEngine;

[HarmonyPatch]
public class Patches
{
    [HarmonyPatch(typeof(Drillable), nameof(Drillable.SpawnLootAsync))]
    [HarmonyPrefix]
    public static void Prefix(Drillable __instance)
    {
        __instance.minResourcesToSpawn = Mathf.Min(Config.Min, Config.Max);
        __instance.maxResourcesToSpawn = Mathf.Max(Config.Min, Config.Max);
    }
}