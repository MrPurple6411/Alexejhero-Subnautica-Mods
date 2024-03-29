﻿namespace MoreModifiedItems.BasicEquipment;

using HarmonyLib;
using MoreModifiedItems.Patchers;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using System.Collections.Generic;
using static CraftData;

internal static class LightweightUltraHighCapacityTank
{
    internal static CustomPrefab Instance { get; set; }

    internal static void CreateAndRegister()
    {
        Instance = new CustomPrefab("lwuhtank", "Lightweight Ultra High Capacity Tank",
            "Has the same amount of oxygen as the Ultra High Capacity Tank, but has the no speed penalty bonus of the Lightweight High Capacity Tank.",
            SpriteManager.Get(TechType.HighCapacityTank));

        Instance.Info.WithSizeInInventory(new Vector2int(3, 4));
        Instance.SetEquipment(EquipmentType.Tank);


        Instance.SetRecipe(new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>()
            {
                new Ingredient(TechType.HighCapacityTank, 1),
                new Ingredient(TechType.PlasteelTank, 1),
                new Ingredient(TechType.Lubricant, 2),
                new Ingredient(TechType.HydrochloricAcid, 1)
            }
        }).WithCraftingTime(5f).WithFabricatorType(CraftTree.Type.Workbench).WithStepsToFabricatorTab("TankMenu".Split('/'));

        if (GetBuilderIndex(TechType.HighCapacityTank, out var group, out var category, out _))
            Instance.SetPdaGroupCategoryAfter(group, category, TechType.HighCapacityTank);

        Instance.SetUnlock(TechType.HighCapacityTank).WithAnalysisTech(null);

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
        EquipmentPatcher.OverrideMap.Add(Instance.Info.TechType, TechType.PlasteelTank);
        Plugin.Log.LogInfo("Lightweight Ultra High Capacity Tank registered");
    }
}
