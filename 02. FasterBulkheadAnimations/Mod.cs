using Harmony;
using ModdingAdventCalendar.Utility;
using System;
using System.Reflection;
using UnityEngine;
using Logger = ModdingAdventCalendar.Utility.Logger;

namespace ModdingAdventCalendar.FasterBulkheadAnimations
{
    public static class QMod
    {
        public static void Patch()
        {
            try
            {
                HarmonyInstance.Create("moddingadventcalendar.fasterbulkheadanimations").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Patching);
            }
        }
    }

    public static class Patches
    {
        [HarmonyPatch(typeof(BulkheadDoor), "SetAnimationState")]
        public static class BulkheadDoor_Awake
        {
            [HarmonyPostfix]
            public static void Postfix(BulkheadDoor __instance)
            {
                AnimationState anim = __instance.GetInstanceField("animState") as AnimationState;
                anim.speed = 3;
            }
        }
    }
}