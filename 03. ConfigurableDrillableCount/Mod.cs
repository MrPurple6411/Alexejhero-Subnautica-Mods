using Harmony;
using ModdingAdventCalendar.Utility;
using SMLHelper.V2.Options;
using SMLHelper.V2.Handlers;
using System;
using System.Reflection;
using UnityEngine;
using Logger = ModdingAdventCalendar.Utility.Logger;

namespace ModdingAdventCalendar.ConfigurableDrillableCount
{
    public static class QMod
    {
        public static string assembly;

        public static void Patch()
        {
            try
            {
                assembly = Assembly.GetExecutingAssembly().GetName().FullName;

                HarmonyInstance.Create("moddingadventcalendar.configurabledrillablecount").PatchAll(Assembly.GetExecutingAssembly());

                Console.WriteLine($"[{assembly}] Patched successfully!");

                CDC.Min = PlayerPrefs.GetInt("cdcMin", 1);
                CDC.Max = PlayerPrefs.GetInt("cdcMax", 3);

                Console.WriteLine($"[{assembly}] Obtained min/max values from config");

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
        [HarmonyPatch(typeof(Drillable), "Start")]
        public static class Drillable_Start
        {
            [HarmonyPostfix]
            public static void Postfix(Drillable __instance)
            {
                try
                {
                    CDC cdc = __instance.gameObject.AddComponent<CDC>();
                    Console.WriteLine($"[{QMod.assembly}] Added component to Drillable!");
                }
                catch (Exception e)
                {
                    Logger.Exception(e, LoggedWhen.InPatch);
                }
            }
        }
    }

    public class CDC : MonoBehaviour
    {
        public static int Min = 1;
        public static int Max = 3;

        public void Start()
        {
            UpdateNumbers();
        }

        public void UpdateNumbers()
        {
            try
            {
                Drillable drillable = gameObject.GetComponent<Drillable>();
                drillable.minResourcesToSpawn = Min;
                drillable.maxResourcesToSpawn = Max;
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }
    }

    public class Options : ModOptions
    {
        public Options(string name) : base(name)
        {
            try
            {
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
                AddSliderOption("cdcMin", "Minimum", 0, 10, CDC.Min);
                AddSliderOption("cdcMax", "Maximum", 0, 10, CDC.Max);
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Options);
            }
        }
    
        public void OnSliderChanged(object sender, SliderChangedEventArgs e)
        {
            try
            {
                int val = (int)Math.Round(e.Value, 0);

                if (e.Id == "cdcMin")
                {
                    Console.WriteLine($"[{QMod.assembly}] Minimum value updated from {CDC.Min} to {val}");
                    CDC.Min = val;
                    PlayerPrefs.SetInt("cdcMin", val);
                }
                else if (e.Id == "cdcMax")
                {
                    Console.WriteLine($"[{QMod.assembly}] Maximum value updated from {CDC.Max} to {val}");
                    CDC.Max = val;
                    PlayerPrefs.SetInt("cdcMax", val);
                }

                UnityEngine.Object.FindObjectsOfType<CDC>().Do(cdc => cdc.UpdateNumbers());
                Console.WriteLine($"[{QMod.assembly}] Updated Drillable components for all objects");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, LoggedWhen.Options);
            }
        }
    }
}