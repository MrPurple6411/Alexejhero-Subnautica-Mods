namespace MoreModifiedItems.BasicEquipment;

using MoreModifiedItems.DeathrunRemade;
using MoreModifiedItems.Patchers;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using System.Collections.Generic;
using static CraftData;

internal static partial class EnhancedStillsuit
{
    internal static CustomPrefab Instance { get; set; }

    internal static void CreateAndRegister()
    {
        Instance = new CustomPrefab("enhancedstillsuit", "Enhanced Water Filtration Suit", "Just like a normal Water Filtration Suit, but it automatically injects the reclaimed water into your system.", SpriteManager.Get(TechType.WaterFiltrationSuit));

        Instance.Info.WithSizeInInventory(new Vector2int(2, 2));
        Instance.SetEquipment(EquipmentType.Body);

        Instance.SetRecipe(new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>()
        {
            new Ingredient(TechType.WaterFiltrationSuit, 1),
            new Ingredient(TechType.ComputerChip, 1),
            new Ingredient(TechType.CopperWire, 2),
            new Ingredient(TechType.Silver, 1),
        }
        }).WithCraftingTime(5f).WithFabricatorType(CraftTree.Type.Workbench).WithStepsToFabricatorTab("BodyMenu".Split('/'));

        if (GetBuilderIndex(TechType.WaterFiltrationSuit, out var group, out var category, out _))
            Instance.SetPdaGroupCategoryAfter(group, category, TechType.WaterFiltrationSuit);

        Instance.SetUnlock(TechType.WaterFiltrationSuit).WithAnalysisTech(null);

        var cloneStillsuit = new CloneTemplate(Instance.Info, TechType.WaterFiltrationSuit)
        {
            ModifyPrefab = (obj) =>
            {
                obj.AddComponent<ESSBehaviour>();
                obj.SetActive(false);
            }
        };

        Instance.SetGameObject(cloneStillsuit);

        Instance.Register();
        EquipmentPatcher.OverrideMap.Add(Instance.Info.TechType, TechType.WaterFiltrationSuit);
        DeathrunCompat.AddSuitCrushDepthMethod(Instance.Info.TechType, new float[] { 500f, 500f });
        Plugin.Log.LogDebug("Enhanced Stillsuit registered");
    }
}
