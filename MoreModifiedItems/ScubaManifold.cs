namespace ScubaManifold;

using HarmonyLib;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Utility;
using System.Collections.Generic;
using System.Linq;
using static CraftData;
using UnityEngine;
using System.Reflection;
using Nautilus.Assets.Gadgets;
using System.IO;

[HarmonyPatch]
internal static class ScubaManifold
{
    private static Texture2D SpriteTexture { get; } = ImageUtils.LoadTextureFromFile($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Assets/ScubaManifold.png");
    internal static CustomPrefab Instance { get; private set; }

    internal static void CreateAndRegister()
    {
        var info = PrefabInfo.WithTechType(classId: "ScubaManifold", displayName: "Scuba Manifold", description: "Combines the oxygen supply of all carried tanks");

        if (SpriteTexture != null)
            info.WithIcon(ImageUtils.LoadSpriteFromTexture(SpriteTexture));
        info.WithSizeInInventory(new Vector2int(3, 2));

        Instance = new CustomPrefab(info);

        if (GetBuilderIndex(TechType.Tank, out var group, out var category, out _))
            Instance.SetPdaGroupCategoryBefore(group, category, TechType.Tank);
        else
            Instance.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);

        Instance.SetRecipe(new RecipeData()
        {
            Ingredients = new List<Ingredient>()
            {
                new Ingredient(TechType.Silicone, 1),
                new Ingredient(TechType.Titanium, 3),
                new Ingredient(TechType.Lubricant, 2)
            },
            craftAmount = 1
        }).WithStepsToFabricatorTab("Personal/Equipment".Split('/'))
        .WithFabricatorType(CraftTree.Type.Fabricator)
        .WithCraftingTime(5f);
        Instance.SetEquipment(EquipmentType.Tank).WithQuickSlotType(QuickSlotType.Passive);

        var cloneTank = new CloneTemplate(info, TechType.Tank)
        {
            ModifyPrefab = (GameObject obj) => GameObject.DestroyImmediate(obj.GetComponent<Oxygen>())
        };

        Instance.SetGameObject(cloneTank);
        Instance.Register();
    }

    public static OxygenManager OxygenManager { get; private set; }
    public static ItemsContainer Container { get; private set; }
    public static Equipment Equipment { get; private set; }

    private const string tankSlot = "Tank";
    private static bool equipped = false;
    private static readonly List<Oxygen> sources = new();

    [HarmonyPatch(typeof(Player), nameof(Player.Start))]
    [HarmonyPostfix]
    private static void Player_Start_Postfix(Player __instance)
    {
        OxygenManager = __instance.oxygenMgr;
        Container = Inventory.main.container;
        Equipment = Inventory.main.equipment;

        List<InventoryItem> items = new();
        Container.GetItemTypes().ForEach(type => items.AddRange(Container.GetItems(type)));

        items.Where(item =>
        {
            Oxygen oxygen = item.item.gameObject.GetComponent<Oxygen>();
            if (oxygen != null)
            {
                sources.Add(oxygen);
                return true;
            }
            return false;
        });

        Container.onAddItem += OnAddItem;
        Container.onRemoveItem += OnRemoveItem;
        Equipment.onEquip += OnEquip;
        Equipment.onUnequip += OnUnequip;

        equipped = Equipment.GetItemInSlot(tankSlot)?.item?.GetTechType() == Instance.Info.TechType;
        if (!equipped) return;

        sources.ForEach(OxygenManager.RegisterSource);
    }

    private static void OnUnequip(string slot, InventoryItem item)
    {
        if (slot != tankSlot || !equipped)
            return;

        equipped = false;
        sources.ForEach(OxygenManager.UnregisterSource);
    }

    private static void OnEquip(string slot, InventoryItem item)
    {
        if (slot != tankSlot)
            return;

        equipped = item?.item?.GetTechType() == Instance.Info.TechType;
        if(equipped)
            sources.ForEach(OxygenManager.RegisterSource);
        else
            sources.ForEach(OxygenManager.UnregisterSource);
    }

    private static void OnRemoveItem(InventoryItem item)
    {
        Oxygen oxygen = item?.item?.gameObject?.GetComponent<Oxygen>();
        if (oxygen != null)
        {
            sources.Remove(oxygen);
            if(equipped)
                OxygenManager.UnregisterSource(oxygen);
        }
    }

    private static void OnAddItem(InventoryItem item)
    {
        Oxygen oxygen = item?.item?.gameObject?.GetComponent<Oxygen>();
        if (oxygen != null)
        {
            sources.Add(oxygen);
            if (equipped)
                OxygenManager.RegisterSource(oxygen);
        }
    }
}
