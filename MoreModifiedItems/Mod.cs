namespace MoreModifiedItems;

using System.Reflection;
using BepInEx;
using HarmonyLib;
using Nautilus.Handlers;
using static BepInEx.Bootstrap.Chainloader;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.ramune.OrganizedWorkbench", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    public static bool OrganizedWorkbench => PluginInfos.ContainsKey("com.ramune.OrganizedWorkbench");

    public void Awake()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);

        if (OrganizedWorkbench)
        {
            CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "TankMenu", "Air Tank Upgrades", SpriteManager.Get(TechType.WaterFiltrationSuit));
            CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "FinsMenu", "Diving Fin Upgrades", SpriteManager.Get(TechType.WaterFiltrationSuit));
            CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "BodyMenu", "Suit Upgrades", SpriteManager.Get(TechType.WaterFiltrationSuit));
        }

        LightweightUltraHighCapacityTank.CreateAndRegister();
        UltraGlideSwimChargeFins.CreateAndRegister();
        EnhancedStillsuit.CreateAndRegister();
        ReinforcedStillsuit.CreateAndRegister();

        Logger.LogInfo("Patched");
    }
}