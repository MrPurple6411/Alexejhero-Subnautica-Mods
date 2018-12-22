using Harmony;
using ModdingAdventCalendar.Utility;
using SMLHelper.V2.Options;
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
                CDC cdc = __instance.gameObject.AddComponent<CDC>();
                cdc.UpdateNumbers();
            }
        }
    }

    public class CDC : MonoBehaviour
    {
        public void Start()
        {
            // OnSliderChange +=
        }

        public void UpdateNumbers()
        {
            Drillable drillable = gameObject.GetComponent<Drillable>();
            // drillable.minResourcesToSpawn =
            // drillable.maxResourcesToSpawn = 
        }
    }

    public class Options : ModOptions
    {
        public Options(string name) : base(name)
        {
            
        }
        public override void BuildModOptions()
        {
            
        }
    }
}