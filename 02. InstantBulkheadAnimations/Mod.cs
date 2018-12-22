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
        public static void Patch()
        {
            try
            {
                HarmonyInstance.Create("moddingadventcalendar.instantbulkheadanimations").PatchAll(Assembly.GetExecutingAssembly());
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