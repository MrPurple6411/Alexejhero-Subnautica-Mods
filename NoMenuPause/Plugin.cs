namespace NoMenuPause;

using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using static BepInEx.Bootstrap.Chainloader;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
public class Plugin: BaseUnityPlugin
{
    #region[Declarations]
    internal static ConfigEntry<bool> _nmp;
    public static bool NMP
    {
        get => !_nmp.Value;
        set => _nmp.Value = value;
    }

    internal static new ManualLogSource Logger;
    #endregion

    private void Awake()
    {
        Logger = base.Logger;
        _nmp = Config.Bind(MyPluginInfo.PLUGIN_GUID, "NMP", false, "Pause while menu is open");
        Harmony.CreateAndPatchAll(typeof(Patches), MyPluginInfo.PLUGIN_GUID);
        Logger.LogInfo("Patched successfully!");

        if (PluginInfos.ContainsKey(Nautilus.PluginInfo.PLUGIN_GUID))
        {
            Logger.LogInfo("Nautilus Found. Initializing In-game Options Menu.");
            Options.Initialize();
        }
        else
        {
            Logger.LogWarning("Nautilus Not Found. In-game Options Menu will not be available.");
        }
    }
}