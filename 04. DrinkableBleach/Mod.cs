using Harmony;
using ModdingAdventCalendar.Utility;
using System;
using System.Reflection;
using UnityEngine;
using Logger = ModdingAdventCalendar.Utility.Logger;

namespace ModdingAdventCalendar.DrinkableBleach
{
    public static class QMod
    {
        public static void Patch()
        {
            try
            {
                HarmonyInstance.Create("moddingadventcalendar.drinkablebleach").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Patching);
            }
        }
    }

    public static class Patches
    {
        [HarmonyPatch(typeof(CraftData), "GetPrefabForTechType")]
        public static class CraftData_GetPrefabForTechType
        {
            [HarmonyPostfix]
            public static void Postfix(GameObject __result, TechType techType)
            {
                try
                {
                    if (techType == TechType.Bleach)
                    {
                        Eatable eatable = __result.AddComponent<Eatable>();
                        eatable.decomposes = false;
                        eatable.despawns = true;
                        eatable.foodValue = -10;
                        eatable.waterValue = -10;

                        Console.WriteLine("Applied Eatable component to Bleach!");
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception(e, LoggedWhen.InPatch);
                }
            }
        }
    }
}