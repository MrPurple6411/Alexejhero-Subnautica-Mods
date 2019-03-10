using AlexejheroYTB.Common;
using Harmony;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;
using System;
using System.Reflection;
using UnityEngine;
using Logger = AlexejheroYTB.Common.Logger;

namespace AlexejheroYTB.InstantBulkheadAnimations
{
    public static class QMod
    {
        public static string assembly = Assembly.GetExecutingAssembly().GetName().Name;

        public static void Patch()
        {
            try
            {
                HarmonyHelper.Patch();

                Logger.Log("Patched successfully!");

                IBA_Config.Enable = PlayerPrefsExtra.GetBool("ibaEnable", true);

                Logger.Log("Obtained values from config");

                OptionsPanelHandler.RegisterModOptions(new Options("Instant Bulkhead Animations"));

                Logger.Log("Registered mod options");
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Initializing);
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
                    if (IBA_Config.Enable)
                    {
                        Vector3 position = Player.main.transform.position;
                        __instance.GetInstanceMethod("ToggleImmediately").Invoke(__instance, null);
                        Player.main.transform.position = position;
                        Logger.Log("Bulkhead animation skipped!", QMod.assembly);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception(e, LoggedWhen.InPatch, QMod.assembly);
                    return false;
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
                AddToggleOption("ibaEnable", "Enable", IBA_Config.Enable);
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
                    if (e.Value) Logger.Log("Enabled mod", QMod.assembly);
                    else Logger.Log("Disabled mod", QMod.assembly);
                    IBA_Config.Enable = e.Value;
                    PlayerPrefsExtra.SetBool("ibaEnable", e.Value);
                    PlayerPrefs.Save();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, LoggedWhen.Options);
            }
        }
    }
}