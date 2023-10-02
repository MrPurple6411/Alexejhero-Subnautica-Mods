namespace ScubaManifold;

using BepInEx;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using static CraftData;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin: BaseUnityPlugin
{
    private static Texture2D SpriteTexture { get; } = ImageUtils.LoadTextureFromFile($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Assets/ScubaManifold.png");

    public void Awake()
    {
        CreateAndRegisterManifold();
        Harmony.CreateAndPatchAll(typeof(ScubaManifoldController), MyPluginInfo.PLUGIN_GUID);
        Logger.LogInfo("Patched!");
    }

    private static void CreateAndRegisterManifold()
    {
        var info = PrefabInfo.WithTechType(classId: "ScubaManifold", displayName: "Scuba Manifold", description: "Combines the oxygen supply of all carried tanks", unlockAtStart: true);
        ScubaManifoldController.ScubaManifoldTechType = info.TechType;

        if (SpriteTexture != null)
            info.WithIcon(ImageUtils.LoadSpriteFromTexture(SpriteTexture));
        info.WithSizeInInventory(new Vector2int(3, 2));

        var scubaManifoldItem = new CustomPrefab(info);

        if (GetBuilderIndex(TechType.Tank, out var group, out var category, out _))
            scubaManifoldItem.SetPdaGroupCategoryBefore(group, category, TechType.Tank);
        else
            scubaManifoldItem.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);

        scubaManifoldItem.SetRecipe(new RecipeData()
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
        scubaManifoldItem.SetEquipment(EquipmentType.Tank).WithQuickSlotType(QuickSlotType.Passive);

        var cloneTank = new CloneTemplate(info, TechType.Tank)
        {
            ModifyPrefab = (GameObject obj) => GameObject.DestroyImmediate(obj.GetComponent<Oxygen>())
        };

        scubaManifoldItem.SetGameObject(cloneTank);
        scubaManifoldItem.Register();
    }
}