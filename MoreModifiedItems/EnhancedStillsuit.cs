namespace MoreModifiedItems;

using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using System.Collections.Generic;
using UnityEngine;
using static CraftData;

[HarmonyPatch]
internal static class EnhancedStillsuit
{
    internal static CustomPrefab Instance { get; set; }

    internal static void CreateAndRegister()
    {
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

        var cloneStillsuit = new CloneTemplate(Instance.Info, TechType.WaterFiltrationSuit)
        {
            ModifyPrefab = (obj) => { obj.AddComponent<ESSBehaviour>(); obj.SetActive(false); }
        };

        Instance.SetGameObject(cloneStillsuit);

        Instance.Register();
    }

    [HarmonyPatch(typeof(Equipment), nameof(Equipment.GetTechTypeInSlot))]
    [HarmonyPostfix]
    public static void Equipment_GetTechTypeInSlot_Postfix(ref TechType __result)
    {
        __result = __result == Instance.Info.TechType || __result == ReinforcedStillsuit.Instance.Info.TechType ? TechType.WaterFiltrationSuit : __result;
    }

    [HarmonyPatch(typeof(Stillsuit), "IEquippable.UpdateEquipped")]
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
                __instance.waterCaptured -= __instance.waterCaptured;
            }
        }

        return false;
    }
}
