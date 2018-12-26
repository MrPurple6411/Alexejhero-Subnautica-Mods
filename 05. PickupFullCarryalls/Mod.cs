using Harmony;
using ModdingAdventCalendar.Utility;
using System;
using System.Reflection;
using UnityEngine;
using Logger = ModdingAdventCalendar.Utility.Logger;

namespace ModdingAdventCalendar.PickupFullCarryalls
{
    public static class QMod
    {
        public static string assembly;

        public static void Patch()
        {
            try
            {
                assembly = Assembly.GetExecutingAssembly().GetName().FullName;

                HarmonyInstance.Create("moddingadventcalendar.pickupfullcarryalls").PatchAll(Assembly.GetExecutingAssembly());

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
        [HarmonyPatch(typeof(PickupableStorage), "OnHandClick")]
        public static class PickupableStorage_OnHandClick
        {
            [HarmonyPrefix]
            public static bool Prefix(PickupableStorage __instance, GUIHand hand)
            {
                try
                {
                    __instance.pickupable.OnHandClick(hand);

                    Console.WriteLine($"[{QMod.assembly}] Picked up a carry-all");
                }
                catch (Exception e)
                {
                    Logger.Exception(e, LoggedWhen.InPatch);
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(PickupableStorage), "OnHandHover")]
        public static class PickupableStorage_OnHandHover
        {
            [HarmonyPrefix]
            public static bool Prefix(PickupableStorage __instance, GUIHand hand)
            {
                try
                {
                    __instance.pickupable.OnHandHover(hand);
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