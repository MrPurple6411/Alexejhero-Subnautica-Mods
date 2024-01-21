namespace MoreModifiedItems.WarpStabilizationSuit;

using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Crafting;
using System.Collections.Generic;
using static CraftData;
using Nautilus.Assets.Gadgets;
using MoreModifiedItems.DeathrunRemade;
using MoreModifiedItems.BasicEquipment;

internal static class StabilizedEnhancedStillsuitMK2
{
    internal static CustomPrefab Instance { get; set; }

    internal static void CreateAndRegister()
    {
        if (!DeathrunCompat.DeathrunLoaded() || !DeathrunCompat.VersionCheck())
            return;

        if (!TechTypeExtensions.FromString("WarpStabilizationSuit", out var warpStabilizationSuit, true) |
            !TechTypeExtensions.FromString("deathrunremade_spineeelscale", out TechType spineeelscale, true))
        {
            return;
        }

        Instance = new CustomPrefab("stabilizedenhancedstillsuitmk2", "Stabilized Enhanced Water Filtration Suit Mk2",
            "An upgraded dive suit capable of protecting the user at depths up to 1300m and providing heat protection up to 75C while also containing the water recycling feature of the Enhanced Water Filtration Suit and protects you from being displaced by teleportation technology",
            SpriteManager.Get(warpStabilizationSuit));

        Instance.Info.WithSizeInInventory(new Vector2int(2, 3));
        Instance.SetEquipment(EquipmentType.Body);

        Instance.SetRecipe(new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>()
            {
                new Ingredient(StabilizedEnhancedStillsuit.Instance.Info.TechType, 1),
                new Ingredient(TechType.AramidFibers, 1),
                new Ingredient(TechType.AluminumOxide, 2),
                new Ingredient(spineeelscale, 2)
            }
        }).WithCraftingTime(5f).WithFabricatorType(CraftTree.Type.Workbench).WithStepsToFabricatorTab("BodyMenu".Split('/'));

        if (GetBuilderIndex(StabilizedEnhancedStillsuit.Instance.Info.TechType, out var group, out var category, out _))
            Instance.SetPdaGroupCategoryAfter(group, category, StabilizedEnhancedStillsuit.Instance.Info.TechType);

        Instance.SetUnlock(warpStabilizationSuit).WithAnalysisTech(null);

        var cloneStillsuit = new CloneTemplate(Instance.Info, TechType.WaterFiltrationSuit)
        {
            ModifyPrefab = (obj) =>
            {
                obj.EnsureComponent<ESSBehaviour>();
                obj.EnsureComponent<AntiWarperBehaviour>();
                obj.SetActive(false);
            }
        };

        Instance.SetGameObject(cloneStillsuit);

        Instance.Register();

        DeathrunCompat.AddSuitCrushDepthMethod(Instance.Info.TechType, new float[] { 1300f });
        DeathrunCompat.AddNitrogenModifierMethod(Instance.Info.TechType, new float[] { 0.25f, 0.2f });
        Plugin.Log.LogDebug("Stabilized Enhanced Stillsuit MK2 registered");
    }
}
