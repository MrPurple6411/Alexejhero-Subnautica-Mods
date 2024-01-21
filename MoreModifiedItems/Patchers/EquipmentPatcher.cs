namespace MoreModifiedItems.Patchers;

using HarmonyLib;
using System.Collections.Generic;

[HarmonyPatch(typeof(Equipment))]
internal static class EquipmentPatcher
{

    internal static Dictionary<TechType, TechType> OverrideMap = new Dictionary<TechType, TechType>();

    [HarmonyPatch(typeof(Equipment), nameof(Equipment.GetTechTypeInSlot))]
    [HarmonyPostfix]
    public static void Equipment_GetTechTypeInSlot_Postfix(Equipment __instance, string slot, ref TechType __result)
    {
        if (!OverrideMap.TryGetValue(__result, out TechType techType))
            return;

        __result = techType;
    }

}
