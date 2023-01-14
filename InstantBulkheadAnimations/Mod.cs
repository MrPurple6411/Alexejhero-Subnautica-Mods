namespace AlexejheroYTB.InstantBulkheadAnimations
{
    using AlexejheroYTB.Common;
    using Harmony;
    using SMLHelper.Handlers;
    using SMLHelper.Options;
    using SMLHelper.Utility;
    using System;
    using UnityEngine;
    using Logger = AlexejheroYTB.Common.Logger;

    public class Plugin
    {
        public void Awake()
        {
            HarmonyHelper.Patch();
            Logger.Log("Patched successfully!");

            IBA_Config.Enable = PlayerPrefsExtra.GetBool("ibaEnable", true);
            Logger.Log("Obtained values from config");

            OptionsPanelHandler.RegisterModOptions(new Options("Instant Bulkhead Animations"));
            Logger.Log("Registered mod options");
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
                if (IBA_Config.Enable)
                {
                    Vector3 position = Player.main.transform.position;
                    __instance.GetInstanceMethod("ToggleImmediately").Invoke(__instance, null);
                    Player.main.transform.position = position;
                    Logger.Log("Bulkhead animation skipped!");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }

    public class IBA_Config
    {
        public static bool Enable = true;
    }

    public class Options : ModOptions
    {
        public Options(string name) : base(name)
        {
            ToggleChanged += OnToggleChanged;
        }

        public override void BuildModOptions()
        {
            AddToggleOption("ibaEnable", "Enable", IBA_Config.Enable);
        }

        public void OnToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id == "ibaEnable")
            {
                if (e.Value) Logger.Log("Enabled mod");
                else Logger.Log("Disabled mod");
                IBA_Config.Enable = e.Value;
                PlayerPrefsExtra.SetBool("ibaEnable", e.Value);
                PlayerPrefs.Save();
            }
        }
    }
}