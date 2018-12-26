using Harmony;
using ModdingAdventCalendar.Utility;
using System;
using System.Reflection;
using UnityEngine;
using Logger = ModdingAdventCalendar.Utility.Logger;

namespace ModdingAdventCalendar.InstantBulkheadAnimations
{
    public static class QMod
    {
        public static string assembly;

        public static void Patch()
        {
            try
            {
                assembly = Assembly.GetExecutingAssembly().GetName().FullName;

                HarmonyInstance.Create("moddingadventcalendar.instantbulkheadanimations").PatchAll(Assembly.GetExecutingAssembly());

                Console.WriteLine($"[{assembly}] Patched successfully!");
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Patching);
            }
        }
    }

    public static class Patches
    {
        [HarmonyPatch(typeof(BulkheadDoor), "OnHandClick")]
        public static class BulkheadDoor_OnHandClick
        {
            [HarmonyPrefix]
            public static bool Prefix(BulkheadDoor __instance, GUIHand hand)
            {
                try
                {
                    Vector3 position = Player.main.transform.position;
                    __instance.GetInstanceMethod("ToggleImmediately").Invoke(__instance, null);
                    Player.main.transform.position = position;
                    Console.WriteLine($"[{QMod.assembly}] Bulkhead animation skipped!");
                }
                catch (Exception e)
                {
                    Logger.Exception(e, LoggedWhen.InPatch);
                }
                return false;
            }
        }
    }
}