using Harmony;
using ModdingAdventCalendar.Utility;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
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
                assembly = Assembly.GetExecutingAssembly().GetName().Name;

                HarmonyInstance.Create("moddingadventcalendar.instantbulkheadanimations").PatchAll(Assembly.GetExecutingAssembly());

                Console.WriteLine($"[{assembly}] Patched successfully!");

                IBA.Enable = PlayerPrefs.GetInt("ibaEnable", 1) == 1 ? true : false;

                Console.WriteLine($"[{assembly}] Obtained values from config");

                OptionsPanelHandler.RegisterModOptions(new Options("Instant Bulkhead Animations"));

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
        [HarmonyPatch(typeof(BulkheadDoor), "OnHandClick")]
        public static class BulkheadDoor_OnHandClick
        {
            [HarmonyPrefix]
            public static bool Prefix(BulkheadDoor __instance, GUIHand hand)
            {
                try
                {
                    if (IBA.Enable)
                    {
                        Vector3 position = Player.main.transform.position;
                        __instance.GetInstanceMethod("ToggleImmediately").Invoke(__instance, null);
                        Player.main.transform.position = position;
                        Console.WriteLine($"[{QMod.assembly}] Bulkhead animation skipped!");
                    }
                    else
                    {
                        Base componentInParent = __instance.GetComponentInParent<Base>();
                        if (componentInParent != null && !componentInParent.isReady)
                        {
                            __instance.GetInstanceMethod("ToggleImmediately").Invoke(__instance, null);
                        }
                        else if (__instance.enabled && (int)__instance.GetInstanceField("state") == 0)
                        {
                            if (GameOptions.GetVrAnimationMode())
                            {
                                __instance.GetInstanceMethod("ToggleImmediately").Invoke(__instance, null);
                            }
                            else
                            {
                                __instance.GetInstanceMethod("SequenceDone").Invoke(__instance, null);
                            }
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

    public class IBA
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
                AddToggleOption("ibaEnable", "Enable", IBA.Enable);
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
                if (e.Id == "ibaEnable")
                {
                    if (e.Value) Console.WriteLine($"[{QMod.assembly}] Enabled mod");
                    else Console.WriteLine($"[{QMod.assembly}] Disabled mod");
                    IBA.Enable = e.Value;
                    PlayerPrefs.SetInt("ibaEnable", e.Value ? 1 : 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, LoggedWhen.Options);
            }
        }
    }
}