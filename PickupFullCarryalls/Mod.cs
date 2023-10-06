namespace PickupableStorageEnhanced;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    public void Awake()
    {
        Logger = base.Logger;

        Harmony.CreateAndPatchAll(typeof(Patches), MyPluginInfo.PLUGIN_GUID);
        Logger.LogInfo("Patched successfully!");

        PFC_Config.Enable = PlayerPrefsExtra.GetBool("pfcEnable", true);
        Logger.LogInfo("Obtained values from config");

        OptionsPanelHandler.RegisterModOptions(new Options("Pickup Full Carry-alls"));
        Logger.LogInfo("Registered mod options");

        ItemActionHandler.RegisterMiddleClickAction(TechType.LuggageBag, InventoryOpener.OnMiddleClick, "open storage", "English", InventoryOpener.Condition);
        ItemActionHandler.RegisterMiddleClickAction(TechType.SmallStorage, InventoryOpener.OnMiddleClick, "open storage", "English", InventoryOpener.Condition);
    }
}
