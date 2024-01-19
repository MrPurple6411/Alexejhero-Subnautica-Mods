﻿namespace MoreModifiedItems;

using HarmonyLib;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Crafting;
using System.Collections.Generic;
using static CraftData;
using Nautilus.Assets.Gadgets;
using BepInEx.Bootstrap;
using System;
using System.Reflection;
using BepInEx;
using MoreModifiedItems.DeathrunRemade;

[HarmonyPatch]
internal static class ReinforcedStillsuit
{
    internal static CustomPrefab Instance { get; set; }

    internal static void CreateAndRegister()
    {
        if (DeathrunCompat.DeathrunLoaded() && !DeathrunCompat.VersionCheck())
        {
            Plugin.Log.LogWarning("Reinforced Stillsuit will not be added because these suits dont work right with Deathrun remade versions below 0.1.5.");
            return;
        }

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

        Instance.SetUnlock(TechType.ReinforcedDiveSuit).WithAnalysisTech(null);

        var cloneStillsuit = new CloneTemplate(Instance.Info, TechType.WaterFiltrationSuit)
        {
            ModifyPrefab = (obj) => { 
                obj.AddComponent<ESSBehaviour>();
                obj.GetComponents<Pickupable>().Do(p => {
                    p.overrideTechType = TechType.ReinforcedDiveSuit;
                    p.overrideTechUsed = true;
                });
                obj.SetActive(false); 
            }
        };

        Instance.SetGameObject(cloneStillsuit);

        Instance.Register();

        DeathrunCompat.AddSuitCrushDepthMethod(Instance.Info.TechType, new float[] { 1300f, 800f });
        DeathrunCompat.AddNitrogenModifierMethod(Instance.Info.TechType, new float[] { 0.25f, 0.2f });

        Plugin.Log.LogDebug("Reinforced Stillsuit registered");
    }
}
