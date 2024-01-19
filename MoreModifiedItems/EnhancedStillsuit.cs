namespace MoreModifiedItems;

using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using MoreModifiedItems.DeathrunRemade;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static CraftData;

[HarmonyPatch]
internal static class EnhancedStillsuit
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
            ModifyPrefab = (obj) => { 
                obj.AddComponent<ESSBehaviour>(); 
                obj.GetComponents<Pickupable>().Do(p =>
                {
                    p.overrideTechType = TechType.WaterFiltrationSuit;
                    p.overrideTechUsed = true;
                });
                obj.SetActive(false); 
            }
        };

        Instance.SetGameObject(cloneStillsuit);

        Instance.Register();

        DeathrunCompat.AddSuitCrushDepthMethod(Instance.Info.TechType, new float[] { 500f, 500f });
        Plugin.Log.LogDebug("Enhanced Stillsuit registered");
    }

    [HarmonyPatch(typeof(Stillsuit), "IEquippable.UpdateEquipped")]
    public static class StillsuitPatch
    {

        [HarmonyPrepare]
        public static bool Stillsuit_UpdateEquipped_Prepare()
        {
            return DeathrunCompat.DeathrunLoaded() && DeathrunCompat.VersionCheck();
        }

        [HarmonyPrefix]
        public static bool Stillsuit_UpdateEquipped_Prefix(Stillsuit __instance)
        {
            if (!__instance.GetComponent<ESSBehaviour>())
            {
                return true;
            }

            Survival survival = Player.main.GetComponent<Survival>();

            if (!survival.freezeStats)
            {
                __instance.waterCaptured += Time.deltaTime / 18f * 0.75f;
                if (__instance.waterCaptured >= 1f)
                {
                    survival.water += __instance.waterCaptured;
                    __instance.waterCaptured = 0;
                }
            }

            return false;
        }
    }
    
}
