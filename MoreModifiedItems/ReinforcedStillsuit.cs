﻿namespace MoreModifiedItems;

using HarmonyLib;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Crafting;
using System.Collections.Generic;
using static CraftData;
using Nautilus.Assets.Gadgets;

[HarmonyPatch]
internal static class ReinforcedStillsuit
{
    internal static CustomPrefab Instance { get; set; }

    internal static void CreateAndRegister()
    {
        Instance = new CustomPrefab("rssuit", "Reinforced Stillsuit", 
            "Offers the same protection as the Reinforced Dive Suit, and also has the water recycling feature of the Enhanced Stillsuit", 
            SpriteManager.Get(TechType.WaterFiltrationSuit));

        Instance.Info.WithSizeInInventory(new Vector2int(2, 3));
        Instance.SetEquipment(EquipmentType.Body);

        Instance.SetRecipe(new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>()
            {
                new Ingredient(TechType.ReinforcedDiveSuit, 1),
                new Ingredient(EnhancedStillsuit.Instance.Info.TechType, 1),
                new Ingredient(TechType.Lubricant, 2),
                new Ingredient(TechType.HydrochloricAcid, 1)
            }
        }).WithCraftingTime(5f).WithFabricatorType(CraftTree.Type.Workbench).WithStepsToFabricatorTab("BodyMenu".Split('/'));

        if (GetBuilderIndex(TechType.WaterFiltrationSuit, out var group, out var category, out _))
            Instance.SetPdaGroupCategoryAfter(group, category, TechType.WaterFiltrationSuit);

        var cloneStillsuit = new CloneTemplate(Instance.Info, TechType.WaterFiltrationSuit)
        {
            ModifyPrefab = (obj) => obj.AddComponent<ESSBehaviour>()
        };

        Instance.SetGameObject(cloneStillsuit);

        Instance.Register();
    }

    [HarmonyPatch(typeof(Equipment), nameof(Equipment.GetTechTypeInSlot))]
    [HarmonyPostfix]
    public static void Equipment_GetTechTypeInSlot_Postfix(ref TechType __result)
    {
        __result = __result == Instance.Info.TechType ? TechType.WaterFiltrationSuit : __result;
    }

    [HarmonyPatch(typeof(Player), nameof(Player.HasReinforcedSuit))]
    public static class Player_HasReinforcedSuit_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            __result = __result || Inventory.main.equipment.GetCount(Instance.Info.TechType) > 0;
        }
    }
}
