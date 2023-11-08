namespace MoreModifiedItems;

using System.Reflection;
using BepInEx;
using HarmonyLib;
using Nautilus.Handlers;
using ScubaManifold;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
public class Plugin : BaseUnityPlugin
{
    public void Awake()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);

        ScubaManifold.CreateAndRegister();

        CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "TankMenu", "Air Tank Upgrades", SpriteManager.Get(TechType.HighCapacityTank));
        LightweightUltraHighCapacityTank.CreateAndRegister();
        
        CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "FinsMenu", "Diving Fin Upgrades", SpriteManager.Get(TechType.UltraGlideFins));
        UltraGlideSwimChargeFins.CreateAndRegister(); 
        
        CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "BodyMenu", "Suit Upgrades", SpriteManager.Get(TechType.WaterFiltrationSuit));
        EnhancedStillsuit.CreateAndRegister();
        ReinforcedStillsuit.CreateAndRegister();

        Logger.LogInfo("Patched");
    }
}