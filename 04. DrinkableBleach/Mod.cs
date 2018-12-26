using Harmony;
using ModdingAdventCalendar.Utility;
using System;
using System.Reflection;
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
        [HarmonyPatch(typeof(WorldForces), "Start")]
        public static class WorldForces_Start
        {
            [HarmonyPrefix]
            public static void Prefix(WorldForces __instance)
            {
                try
                {
                    TechTag tag = __instance.gameObject.GetComponent<TechTag>();

                    if (tag == null)
                    {
                        Console.WriteLine($"{__instance.gameObject.name} doesn't have a TechTag!");
                        return;
                    }

                    if (tag.type != TechType.Bleach) return;

                    Eatable eatable = __instance.gameObject.AddComponent<Eatable>();
                    eatable.decomposes = false;
                    eatable.despawns = true;
                    eatable.foodValue = -10;
                    eatable.waterValue = -10;

                    Console.WriteLine("Applied eatable component to bleach!");
                }
                catch (Exception e)
                {
                    Logger.Exception(e, LoggedWhen.InPatch);
                }
            }
        }
    }
}