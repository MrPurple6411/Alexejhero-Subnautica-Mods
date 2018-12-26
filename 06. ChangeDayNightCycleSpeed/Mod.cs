using Harmony;
using ModdingAdventCalendar.Utility;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using System;
using System.Reflection;
using UnityEngine;
using Logger = ModdingAdventCalendar.Utility.Logger;

namespace ModdingAdventCalendar.ChangeDayNightCycleSpeed
{
    public static class QMod
    {
        public static string assembly;

        public static void Patch()
        {
            try
            {
                assembly = Assembly.GetExecutingAssembly().GetName().Name;

                HarmonyInstance.Create("moddingadventcalendar.changedaynightcyclespeed").PatchAll(Assembly.GetExecutingAssembly());

                Console.WriteLine($"[{assembly}] Patched successfully!");

                CDNCS.Enable = PlayerPrefs.GetInt("cdncsEnable", 1) == 1 ? true : false;

                Console.WriteLine($"[{assembly}] Obtained values from config");

                OptionsPanelHandler.RegisterModOptions(new Options("Change Day Night Cycle Speed"));

                Console.WriteLine($"[{assembly}] Registered mod options");
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Patching);
            }
        }
    }

    public class CDNCS
    {
        public static bool Enable = true;
        public static float Multiplier
        {
            get
            {
                try
                {
                    return (float)DayNightCycle.main.GetInstanceField("_dayNightSpeed");
                }
                catch (Exception e)
                {
                    Logger.Exception(e);
                    return 1;
                }
            }
            set
            {
                try
                {
                    DayNightCycle.main.SetInstanceField("_dayNightSpeed", Mathf.Clamp(value, 0f, 100f));
                }
                catch (Exception e)
                {
                    Logger.Exception(e);
                }
            }
        }
    }

    public class Options : ModOptions
    {
        public Options(string name) : base(name)
        {
            try
            {
                ToggleChanged += OnToggleChanged;
                SliderChanged += OnSliderChanged;
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
                AddSliderOption("cdncsMultiplier", "Day / Night Cycle Multiplier", 0, 100, CDNCS.Multiplier);
                AddToggleOption("cdncsEnable", "Enable", CDNCS.Enable);
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
                if (e.Id == "cdncsEnable")
                {
                    if (e.Value) Console.WriteLine($"[{QMod.assembly}] Enabled mod");
                    else Console.WriteLine($"[{QMod.assembly}] Disabled mod");
                    CDNCS.Enable = e.Value;
                    PlayerPrefs.SetInt("cdncsEnable", e.Value ? 1 : 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, LoggedWhen.Options);
            }
        }

        public void OnSliderChanged(object sender, SliderChangedEventArgs e)
        {
            try
            {
                if (e.Id == "cdncsMultiplier")
                {
                    Console.WriteLine($"[{QMod.assembly}] Multiplier updated from {CDNCS.Multiplier} to {e.Value}");
                    CDNCS.Multiplier = e.Value;
                    PlayerPrefs.SetFloat("cdncsMax", e.Value);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, LoggedWhen.Options);
            }
        }
    }
}