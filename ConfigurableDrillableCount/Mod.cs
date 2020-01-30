using AlexejheroYTB.Common;
using Harmony;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using System;
using System.Reflection;
using UnityEngine;
using Logger = AlexejheroYTB.Common.Logger;

namespace AlexejheroYTB.ConfigurableDrillableCount
{
    public static class QMod
    {
        public static void Patch()
        {
            HarmonyHelper.Patch();

            Logger.Log($"Patched successfully!");

            CDC_Config.Min = PlayerPrefs.GetInt("cdcMin", 1);
            CDC_Config.Max = PlayerPrefs.GetInt("cdcMax", 3);

            Logger.Log("Obtained min/max values from config");

            OptionsPanelHandler.RegisterModOptions(new Options("Configurable Drillable Count"));

            Logger.Log("Registered mod options");
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
                CDC_Config cdc = __instance.gameObject.AddComponent<CDC_Config>();
                Logger.Log("Added component to Drillable!");
            }
        }
    }

    public class CDC_Config : MonoBehaviour
    {
        public static int Min = 1;
        public static int Max = 3;

        public void Start()
        {
            UpdateNumbers();
        }

        public void UpdateNumbers()
        {
            Drillable drillable = gameObject.EnsureComponent<Drillable>();
            drillable.minResourcesToSpawn = Min;
            drillable.maxResourcesToSpawn = Max;
        }
    }

    public class Options : ModOptions
    {
        public Options(string name) : base(name)
        {
            SliderChanged += OnSliderChanged;
        }

        public override void BuildModOptions()
        {
            AddSliderOption("cdcMin", "Minimum", 0, 10, CDC_Config.Min);
            AddSliderOption("cdcMax", "Maximum", 0, 10, CDC_Config.Max);
        }

        public void OnSliderChanged(object sender, SliderChangedEventArgs e)
        {
            int val = (int)Math.Round(e.Value, 0);

            if (e.Id == "cdcMin")
            {
                Logger.Log($"Minimum value updated from {CDC_Config.Min} to {val}");
                CDC_Config.Min = val;
                PlayerPrefs.SetInt("cdcMin", val);
                PlayerPrefs.Save();
            }
            else if (e.Id == "cdcMax")
            {
                Logger.Log($"Maximum value updated from {CDC_Config.Max} to {val}");
                CDC_Config.Max = val;
                PlayerPrefs.SetInt("cdcMax", val);
                PlayerPrefs.Save();
            }

            UnityEngine.Object.FindObjectsOfType<CDC_Config>().Do(cdc => cdc.UpdateNumbers());
            Logger.Log("Updated Drillable components for all objects");
        }
    }
}