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
                assembly = Assembly.GetExecutingAssembly().GetName().Name;

                HarmonyInstance.Create("moddingadventcalendar.pickupfullcarryalls").PatchAll(Assembly.GetExecutingAssembly());

                Console.WriteLine($"[{assembly}] Patched successfully!");

                PFC.Enable = PlayerPrefs.GetInt("pfcEnable", 1) == 1 ? true : false;

                Console.WriteLine($"[{assembly}] Obtained values from config");

                OptionsPanelHandler.RegisterModOptions(new Options("Pickup Full Carry-alls"));

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
                    if (PFC.Enable)
                    {
                        __instance.pickupable.OnHandHover(hand);
                    }
                    else
                    {
                        if (__instance.storageContainer.IsEmpty())
                        {
                            __instance.pickupable.OnHandHover(hand);
                        }
                        else if (!string.IsNullOrEmpty(__instance.cantPickupHoverText))
                        {
#pragma warning disable CS0618 // Type or member is obsolete
                            HandReticle.main.SetInteractText(__instance.cantPickupHoverText, string.Empty, true, false, false);
#pragma warning restore CS0618 // Type or member is obsolete
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
                AddToggleOption("pfcEnable", "Enable", PFC.Enable);
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