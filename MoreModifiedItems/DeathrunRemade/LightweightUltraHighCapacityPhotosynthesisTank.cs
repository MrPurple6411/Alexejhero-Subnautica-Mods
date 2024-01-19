namespace MoreModifiedItems.DeathrunRemade;

using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System.Collections.Generic;
using static CraftData;

[HarmonyPatch]
internal static class LightweightUltraHighCapacityPhotosynthesisTank
{
    internal static CustomPrefab Instance { get; set; }
    private static TechType photosynthesistank;

    internal static void CreateAndRegister()
    {
        if (!EnumHandler.TryGetValue("photosynthesistank", out photosynthesistank) && !EnumHandler.TryGetValue("deathrunremade_photosynthesistank", out photosynthesistank))
        {
            return;
        }

        Instance = new CustomPrefab("lwuhcptank", "Lightweight Ultra High Capacity Photosynthesis Tank",
            "Has all the benefits of the Lightweight Ultra High Capacity and the 4x oxygen production of the Photosynthesis Tank",
            SpriteManager.Get(TechType.HighCapacityTank));

        Instance.Info.WithSizeInInventory(new Vector2int(3, 4));
        Instance.SetEquipment(EquipmentType.Tank);

        Instance.SetRecipe(new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>()
            {
                new Ingredient(TechType.HighCapacityTank, 1),
                new Ingredient(photosynthesistank, 1),
                new Ingredient(TechType.Lubricant, 2),
                new Ingredient(TechType.HydrochloricAcid, 1)
            }
        }).WithCraftingTime(5f).WithFabricatorType(CraftTree.Type.Workbench).WithStepsToFabricatorTab("TankMenu".Split('/'));

        if (GetBuilderIndex(photosynthesistank, out var group, out var category, out _))
            Instance.SetPdaGroupCategoryAfter(group, category, photosynthesistank);

        Instance.SetUnlock(photosynthesistank).WithAnalysisTech(null);

        var clonetank = new CloneTemplate(Instance.Info, TechType.HighCapacityTank)
        {
            ModifyPrefab = (obj) =>
            {
                obj.GetAllComponentsInChildren<Oxygen>().Do(o => o.oxygenCapacity = 180);
                obj.GetComponentsInChildren<Pickupable>().Do(p =>
                {
                    p.overrideTechType = photosynthesistank;
                    p.overrideTechUsed = true;
                });
                obj.SetActive(false);
            }
        };

        Instance.SetGameObject(clonetank);

        Instance.Register();
    }

    [HarmonyPatch(typeof(Equipment), nameof(Equipment.GetTechTypeInSlot))]
    [HarmonyPostfix]
    public static void Equipment_GetTechTypeInSlot_Postfix(Equipment __instance, string slot, ref TechType __result)
    {
        if (slot != "Tank" || photosynthesistank == TechType.None || __result != photosynthesistank)
            return;

        InventoryItem itemInSlot = __instance.GetItemInSlot(slot);
        if (itemInSlot == null)
            return;

        Pickupable item = itemInSlot.item;
        if (item == null)
            return;

        PrefabIdentifier prefabIdentifier = item.GetComponent<PrefabIdentifier>();
        if (prefabIdentifier == null)
            return;

        if (prefabIdentifier.ClassId == Instance.Info.ClassID)
            __result = TechType.PlasteelTank;
    }
}
