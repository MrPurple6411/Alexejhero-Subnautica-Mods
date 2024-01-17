namespace MoreModifiedItems;

using BepInEx.Bootstrap;
using HarmonyLib;
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
        // TODO: Remove this check when Deathrun Remade is updated with the new NitrogenHandler and HeatHandler api's
        if (Chainloader.PluginInfos.ContainsKey("com.github.tinyhoot.DeathrunRemade"))
        {
            Plugin.Log.LogWarning("Enhanced Stillsuit will not be added because these suits dont work right with Deathrun. Waiting on Nitrogen and Heat API's");
            return;
        }

        Instance = new CustomPrefab("enhancedstillsuit", "Enhanced Stillsuit", "Just like a normal stillsuit, but it automatically injects the reclaimed water into your system.", SpriteManager.Get(TechType.WaterFiltrationSuit));

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

        if (!Chainloader.PluginInfos.ContainsKey("com.github.tinyhoot.DeathrunRemade"))
            return;

        // Deathrun Remade compatibility

        Type crushDepthHandler = AccessTools.TypeByName("DeathrunRemade.DeathrunAPI");

        if (crushDepthHandler == null)
        {
            Plugin.Log.LogError("Failed to get CrushDepthHandler type.");
            return;
        }

        MethodInfo AddSuitCrushDepth = AccessTools.Method(crushDepthHandler, "AddSuitCrushDepth", new Type[] { typeof(TechType), typeof(IEnumerable<float>) });

        if (AddSuitCrushDepth == null)
        {
            Plugin.Log.LogError("Failed to get AddSuitCrushDepth method.");
            return;
        }

        float[] depths = new float[] { 1300f, 800f };

        AddSuitCrushDepth.Invoke(null, new object[] { Instance.Info.TechType, depths });

    }

    [HarmonyPatch(typeof(Stillsuit), "IEquippable.UpdateEquipped")]
    [HarmonyPrefix]
    public static bool Stillsuit_UpdateEquipped_Prefix(Stillsuit __instance)
    {
        // TODO: Remove this check when Deathrun Remade is updated with the new NitrogenHandler and HeatHandler api's
        if (Chainloader.PluginInfos.ContainsKey("com.github.tinyhoot.DeathrunRemade"))
        {
            return true;
        }

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
