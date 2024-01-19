namespace MoreModifiedItems;

using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using MoreModifiedItems.DeathrunRemade;
using Nautilus.Handlers;
using ScubaManifold;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.github.tinyhoot.DeathrunRemade", BepInDependency.DependencyFlags.SoftDependency)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource Log { get; private set; }
    internal static Harmony harmony { get; private set; } = new Harmony(MyPluginInfo.PLUGIN_GUID);

    public void Awake()
    {
        Log = Logger;

        harmony.PatchAll(Assembly.GetExecutingAssembly());

        ScubaManifold.CreateAndRegister();

        if (Chainloader.PluginInfos.ContainsKey("com.github.tinyhoot.DeathrunRemade") && TechTypeExtensions.FromString("deathrunremade_photosynthesistanksmall", out TechType smallTank, true))
        {
            CraftTreeHandler.RemoveNode(CraftTree.Type.Workbench, "deathrunremade_specialtanks", smallTank.ToString());
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, smallTank, "Personal", "Equipment");
        }

        CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "TankMenu", "Air Tank Upgrades", SpriteManager.Get(TechType.HighCapacityTank));
        LightweightUltraHighCapacityTank.CreateAndRegister();
        LightweightUltraHighCapacityPhotosynthesisTank.CreateAndRegister();
        LightweightUltraHighCapacityChemosynthesisTank.CreateAndRegister();
        
        CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "FinsMenu", "Diving Fin Upgrades", SpriteManager.Get(TechType.UltraGlideFins));
        UltraGlideSwimChargeFins.CreateAndRegister(); 
        
        CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "BodyMenu", "Suit Upgrades", SpriteManager.Get(TechType.WaterFiltrationSuit));

        DeathrunCompat.AddSuitCrushDepthMethod(TechType.WaterFiltrationSuit, new float[] { 500f, 500f });
        DeathrunCompat.AddNitrogenModifierMethod(TechType.ReinforcedDiveSuit, new float[] { 0.25f, 0.2f });

        EnhancedStillsuit.CreateAndRegister();
        ReinforcedStillsuit.CreateAndRegister();
        ReinforcedStillsuitMK2.CreateAndRegister();
        ReinforcedStillsuitMK3.CreateAndRegister();

        DeathrunCompat.PatchDeathrunTank();

        Logger.LogInfo("Patched");
    }
}