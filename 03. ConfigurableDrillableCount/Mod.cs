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
        public static void Patch()
        {
            try
            {
                HarmonyInstance.Create("moddingadventcalendar.configurabledrillablecount").PatchAll(Assembly.GetExecutingAssembly());

                CDC.Min = PlayerPrefs.GetInt("cdcMin", 1);
                CDC.Max = PlayerPrefs.GetInt("cdcMax", 3);

                OptionsPanelHandler.RegisterModOptions(new Options("Configurable Drillable Count"));
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
                    CDC.Min = val;
                    PlayerPrefs.SetInt("cdcMin", val);
                }
                else if (e.Id == "cdcMax")
                {
                    CDC.Max = val;
                    PlayerPrefs.SetInt("cdcMax", val);
                }

                UnityEngine.Object.FindObjectsOfType<CDC>().Do(cdc => cdc.UpdateNumbers());
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, LoggedWhen.Options);
            }
        }
    }
}