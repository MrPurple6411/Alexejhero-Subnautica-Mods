namespace MoreModifiedItems.BasicEquipment;

using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Utility;
using System.Collections.Generic;
using static CraftData;
using UnityEngine;
using System.Reflection;
using Nautilus.Assets.Gadgets;
using System.IO;

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

        Instance.SetUnlock(TechType.Rebreather).WithAnalysisTech(null);

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
            ModifyPrefab = (obj) => { Object.DestroyImmediate(obj.GetComponent<Oxygen>()); obj.SetActive(false); }
        };

        Instance.SetGameObject(cloneTank);
        Instance.Register();
        Plugin.Log.LogInfo("Scuba Manifold registered");
    }
}
