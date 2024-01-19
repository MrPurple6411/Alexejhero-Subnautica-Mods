namespace MoreModifiedItems;

using HarmonyLib;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Crafting;
using System.Collections.Generic;
using static CraftData;
using Nautilus.Assets.Gadgets;
using MoreModifiedItems.DeathrunRemade;

[HarmonyPatch]
internal static class ReinforcedStillsuitMK3
{
    internal static CustomPrefab Instance { get; set; }

    internal static void CreateAndRegister()
    {
        if (!TechTypeExtensions.FromString("deathrunremade_reinforcedsuit3", out TechType reinforcedsuit3, true) |
            !TechTypeExtensions.FromString("rssuitmk2", out TechType rssuitmk2, true) |
            !TechTypeExtensions.FromString("deathrunremade_lavalizardscale", out TechType lavalizardscale, true))
        {
            Plugin.Log.LogError($"Failed to load Reinforced Water Filtration Suit Mk3 - {reinforcedsuit3}, {rssuitmk2}, {lavalizardscale}");
            return;
        }

        Instance = new CustomPrefab("rssuitmk3", "Reinforced Enhanced Water Filtration Suit Mk3",
            "An upgraded dive suit capable of protecting the user at all depths and providing heat protection up to 90C while also containing the water recycling feature of the Enhanced Water Filtration Suit",
            SpriteManager.Get(reinforcedsuit3));

        Instance.Info.WithSizeInInventory(new Vector2int(2, 3));
        Instance.SetEquipment(EquipmentType.Body);

        Instance.SetRecipe(new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>()
            {
                new Ingredient(rssuitmk2, 1),
                new Ingredient(TechType.AramidFibers, 1),
                new Ingredient(TechType.Kyanite, 2),
                new Ingredient(lavalizardscale, 2)
            }
        }).WithCraftingTime(5f).WithFabricatorType(CraftTree.Type.Workbench).WithStepsToFabricatorTab("BodyMenu".Split('/'));

        if (GetBuilderIndex(rssuitmk2, out var group, out var category, out _))
            Instance.SetPdaGroupCategoryAfter(group, category, rssuitmk2);

        Instance.SetUnlock(reinforcedsuit3).WithAnalysisTech(null);

        var cloneStillsuit = new CloneTemplate(Instance.Info, reinforcedsuit3)
        {
            ModifyPrefab = (obj) =>
            {
                obj.SetActive(false);
                obj.EnsureComponent<ESSBehaviour>();
                obj.GetComponents<Pickupable>().Do(p =>
                {
                    p.overrideTechType = reinforcedsuit3;
                    p.overrideTechUsed = true;
                });
            }
        };

        Instance.SetGameObject(cloneStillsuit);

        Instance.Register();

        DeathrunCompat.AddSuitCrushDepthMethod(Instance.Info.TechType, new float[] { 10000f });
        DeathrunCompat.AddNitrogenModifierMethod(Instance.Info.TechType, new float[] { 0.45f, 0.3f });

        Plugin.Log.LogDebug("Reinforced Stillsuit MKII registered");
    }
}
