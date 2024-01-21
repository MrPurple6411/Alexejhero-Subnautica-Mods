namespace MoreModifiedItems.Patchers;

using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(Stillsuit))]
internal static class StillsuitPatcher
{
    [HarmonyPatch("IEquippable.UpdateEquipped")]
    [HarmonyPrefix]
    public static bool Stillsuit_UpdateEquipped_Prefix(Stillsuit __instance)
    {
        if (!__instance.GetComponent<ESSBehaviour>())
        {
            return true;
        }

        Survival survival = Player.main.GetComponent<Survival>();

        if (!survival.freezeStats)
        {
            __instance.waterCaptured += Time.deltaTime / 18f * 0.75f;
            if (__instance.waterCaptured >= 1f)
            {
                survival.water += __instance.waterCaptured;
                __instance.waterCaptured = 0;
            }
        }

        return false;
    }
}
