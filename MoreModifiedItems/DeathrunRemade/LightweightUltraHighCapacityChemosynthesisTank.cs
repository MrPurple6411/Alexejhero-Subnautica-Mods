namespace MoreModifiedItems.DeathrunRemade;

using HarmonyLib;
using MoreModifiedItems.Patchers;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System.Collections.Generic;
using static CraftData;

internal static class LightweightUltraHighCapacityChemosynthesisTank
{
    internal static CustomPrefab Instance { get; set; }
    private static TechType chemosynthesistank;

    internal static void CreateAndRegister()
    {
        if (!EnumHandler.TryGetValue("chemosynthesistank", out chemosynthesistank) && !EnumHandler.TryGetValue("deathrunremade_chemosynthesistank", out chemosynthesistank))
        {
            return;
        }

        Instance = new CustomPrefab("lwuhcctank", "Lightweight Ultra High Capacity Chemosynthesis Tank",
            "Has all the benefits of the Lightweight Ultra High Capacity and the 4x the oxygen production of the Chemosynthesis Tank",
            SpriteManager.Get(TechType.HighCapacityTank));

        Instance.Info.WithSizeInInventory(new Vector2int(3, 4));
        Instance.SetEquipment(EquipmentType.Tank);



        Instance.SetRecipe(new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>()
            {
                new Ingredient(TechType.HighCapacityTank, 1),
                new Ingredient(chemosynthesistank, 1),
                new Ingredient(TechType.Lubricant, 2),
                new Ingredient(TechType.HydrochloricAcid, 1)
            }
        }).WithCraftingTime(5f).WithFabricatorType(CraftTree.Type.Workbench).WithStepsToFabricatorTab("TankMenu".Split('/'));

        if (GetBuilderIndex(chemosynthesistank, out var group, out var category, out _))
            Instance.SetPdaGroupCategoryAfter(group, category, chemosynthesistank);

        Instance.SetUnlock(chemosynthesistank).WithAnalysisTech(null);

        var clonetank = new CloneTemplate(Instance.Info, TechType.HighCapacityTank)
        {
            ModifyPrefab = (obj) =>
            {
                obj.GetAllComponentsInChildren<Oxygen>().Do(o => o.oxygenCapacity = 180);
                obj.SetActive(false);
            }
        };

        Instance.SetGameObject(clonetank);
        Instance.Register();
        EquipmentPatcher.OverrideMap.Add(Instance.Info.TechType, chemosynthesistank);
        Plugin.Log.LogInfo("Lightweight Ultra High Capacity Chemosynthesis Tank registered");
    }
}
