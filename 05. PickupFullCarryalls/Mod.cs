using Harmony;
using ModdingAdventCalendar.Utility;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
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

                PFC.Enable = PlayerPrefs.GetInt("pfcEnable", 1) == 1 ? true : false;

                Console.WriteLine($"[{assembly}] Obtained values from config");

                OptionsPanelHandler.RegisterModOptions(new Options("Configurable Drillable Count"));

                Console.WriteLine($"[{assembly}] Registered mod options");
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
                    if (PFC.Enable)
                    {
                        __instance.pickupable.OnHandClick(hand);
                        Console.WriteLine($"[{QMod.assembly}] Picked up a carry-all");
                    }
                    else
                    {
                        if (__instance.storageContainer.IsEmpty())
                        {
                            __instance.pickupable.OnHandClick(hand);
                        }
                        else if (!string.IsNullOrEmpty(__instance.cantPickupClickText))
                        {
                            ErrorMessage.AddError(Language.main.Get(__instance.cantPickupClickText));
                        }
                    }
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

    public class PFC
    {
        public static bool Enable = true;
    }

    public class Options : ModOptions
    {
        public Options(string name) : base(name)
        {
            try
            {
                ToggleChanged += OnToggleChanged;
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Options);
            }
        }

        public override void BuildModOptions()
        {
            try
            {
                AddToggleOption("pfcEnable", "Enable", true);
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Options);
            }
        }

        public void OnToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            try
            {
                if (e.Id == "pfcEnable")
                {
                    if (e.Value) Console.WriteLine($"[{QMod.assembly}] Enabled mod");
                    else Console.WriteLine($"[{QMod.assembly}] Disabled mod");
                    PFC.Enable = e.Value;
                    PlayerPrefs.SetInt("pfcEnable", e.Value ? 1 : 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, LoggedWhen.Options);
            }
        }
    }
}