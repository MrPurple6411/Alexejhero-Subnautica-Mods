namespace MoreModifiedItems.WarpStabilizationSuit;

using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Crafting;
using System.Collections.Generic;
using static CraftData;
using Nautilus.Assets.Gadgets;
using MoreModifiedItems.DeathrunRemade;

internal static class StabilizedEnhancedStillsuitMK3
{
    internal static CustomPrefab Instance { get; set; }

    internal static void CreateAndRegister()
    {
        if (!DeathrunCompat.DeathrunLoaded() || !DeathrunCompat.VersionCheck())
            return;

        if (!TechTypeExtensions.FromString("WarpStabilizationSuit", out var warpStabilizationSuit, true) |
            !TechTypeExtensions.FromString("deathrunremade_lavalizardscale", out TechType lavalizardscale, true))
        {
            return;
        }

        Instance = new CustomPrefab("stabilizedenhancedstillsuitmk3", "Stabilized Enhanced Water Filtration Suit Mk3",
            "An upgraded dive suit capable of protecting the user at all depths and providing heat protection up to 90C while also containing the water recycling feature of the Enhanced Water Filtration Suit and protects you from being displaced by teleportation technology",
            SpriteManager.Get(warpStabilizationSuit));

        Instance.Info.WithSizeInInventory(new Vector2int(2, 3));
        Instance.SetEquipment(EquipmentType.Body);

        Instance.SetRecipe(new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>()
            {
                new Ingredient(StabilizedEnhancedStillsuitMK2.Instance.Info.TechType, 1),
                new Ingredient(TechType.AramidFibers, 1),
                new Ingredient(TechType.Kyanite, 2),
                new Ingredient(lavalizardscale, 2)
            }
        }).WithCraftingTime(5f).WithFabricatorType(CraftTree.Type.Workbench).WithStepsToFabricatorTab("BodyMenu".Split('/'));

        if (GetBuilderIndex(StabilizedEnhancedStillsuitMK2.Instance.Info.TechType, out var group, out var category, out _))
            Instance.SetPdaGroupCategoryAfter(group, category, StabilizedEnhancedStillsuitMK2.Instance.Info.TechType);

        Instance.SetUnlock(StabilizedEnhancedStillsuitMK2.Instance.Info.TechType).WithAnalysisTech(null);

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

        DeathrunCompat.AddSuitCrushDepthMethod(Instance.Info.TechType, new float[] { 10000f });
        DeathrunCompat.AddNitrogenModifierMethod(Instance.Info.TechType, new float[] { 0.45f, 0.3f });

        Plugin.Log.LogDebug("Stabilized Enhanced Stillsuit Mk3 registered");
    }
}
