namespace ConfigurableDrillableCount;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using static BepInEx.Bootstrap.Chainloader;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    public void Awake()
    {
        Logger = base.Logger;

        Harmony.CreateAndPatchAll(typeof(Patches), MyPluginInfo.PLUGIN_GUID);
        Logger.LogInfo($"Patched successfully!");

        CDC_Config.Min = PlayerPrefs.GetInt("cdcMin", 1);
        CDC_Config.Max = PlayerPrefs.GetInt("cdcMax", 3);
        Logger.LogInfo("Obtained min/max values from config");

        if (PluginInfos.ContainsKey("com.snmodding.nautilus"))
        {
            Logger.LogInfo("Nautilus Found. Initializing In-game Options Menu.");
            AccessTools.Method("ConfigurableDrillableCount.Options:Initialize")?.Invoke(null, null);
            Logger.LogInfo("Registered mod options");
        }
    }
}