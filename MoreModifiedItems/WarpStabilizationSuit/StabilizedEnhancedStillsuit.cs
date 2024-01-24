namespace MoreModifiedItems.WarpStabilizationSuit;

using MoreModifiedItems.BasicEquipment;
using MoreModifiedItems.DeathrunRemade;
using MoreModifiedItems.Patchers;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System.Collections.Generic;
using static CraftData;

internal static class StabilizedEnhancedStillsuit
{
    internal static CustomPrefab Instance { get; set; }

    internal static void CreateAndRegister()
    {
        if (!TechTypeExtensions.FromString("WarpStabilizationSuit", out var warpStabilizationSuit, true))
        {
            return;
        }

        Plugin.Log.LogInfo("WarpStabilizationSuit found. Creating new Suits.");

        CraftTreeHandler.RemoveNode(CraftTree.Type.Workbench, "ModdedWorkbench", warpStabilizationSuit.AsString());
        CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, warpStabilizationSuit, "BodyMenu".Split('/'));

        DeathrunCompat.AddSuitCrushDepthMethod(warpStabilizationSuit, new float[] { 1300f, 800f });
        DeathrunCompat.AddNitrogenModifierMethod(warpStabilizationSuit, new float[] { 0.25f, 0.2f });

        Instance = new CustomPrefab("stabilizedenhancedstillsuit", "Stabilized Enhanced Water Filtration Suit", "Enhanced Water Filtration Suit, but it protects you from being displaced by teleportation technology.", 
            SpriteManager.Get(warpStabilizationSuit));

        Instance.Info.WithSizeInInventory(new Vector2int(2, 2));
        Instance.SetEquipment(EquipmentType.Body);

        Instance.SetRecipe(new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>()
        {
            new Ingredient(EnhancedStillsuit.Instance.Info.TechType, 1),
            new Ingredient(warpStabilizationSuit, 1),
        }
        }).WithCraftingTime(5f).WithFabricatorType(CraftTree.Type.Workbench).WithStepsToFabricatorTab("BodyMenu".Split('/'));

        if (GetBuilderIndex(warpStabilizationSuit, out var group, out var category, out _))
            Instance.SetPdaGroupCategoryAfter(group, category, warpStabilizationSuit);

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
        DeathrunCompat.AddSuitCrushDepthMethod(Instance.Info.TechType, new float[] { 1300f, 800f });
        DeathrunCompat.AddNitrogenModifierMethod(Instance.Info.TechType, new float[] { 0.25f, 0.2f });
        Plugin.Log.LogDebug("Stabilized Enhanced Stillsuit registered");
    }
}
