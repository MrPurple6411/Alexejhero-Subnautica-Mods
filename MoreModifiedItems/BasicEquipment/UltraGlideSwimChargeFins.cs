namespace MoreModifiedItems.BasicEquipment;

using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Crafting;
using System.Collections.Generic;
using static CraftData;
using Nautilus.Assets.Gadgets;
using MoreModifiedItems.Patchers;

internal static class UltraGlideSwimChargeFins
{
    internal static CustomPrefab Instance { get; set; }

    internal static void CreateAndRegister()
    {
        Instance = new CustomPrefab("ugscfins", "Ultra Glide Swim Charge Fins",
            "Has the same speed increase as the Ultra Glide Fins, but also has the tool recharge ability of the Swim Charge Fins.",
            SpriteManager.Get(TechType.SwimChargeFins));
        Instance.Info.WithSizeInInventory(new Vector2int(2, 3));
        Instance.SetEquipment(EquipmentType.Foots);

        Instance.SetRecipe(new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>()
            {
                new Ingredient(TechType.UltraGlideFins, 1),
                new Ingredient(TechType.SwimChargeFins, 1),
                new Ingredient(TechType.Lubricant, 2),
                new Ingredient(TechType.HydrochloricAcid, 1)
            }
        }).WithCraftingTime(5f).WithFabricatorType(CraftTree.Type.Workbench).WithStepsToFabricatorTab("FinsMenu".Split('/'));

        if (GetBuilderIndex(TechType.UltraGlideFins, out var group, out var category, out _))
            Instance.SetPdaGroupCategoryAfter(group, category, TechType.UltraGlideFins);

        Instance.SetUnlock(TechType.UltraGlideFins).WithAnalysisTech(null);

        var cloneStillsuit = new CloneTemplate(Instance.Info, TechType.UltraGlideFins)
        {
            ModifyPrefab = (obj) =>
            {
                obj.SetActive(false);
            }
        };

        Instance.SetGameObject(cloneStillsuit);

        Instance.Register();
        EquipmentPatcher.OverrideMap.Add(Instance.Info.TechType, TechType.UltraGlideFins);
        TechType = Instance.Info.TechType;
        Plugin.Log.LogInfo("Ultra Glide Swim Charge Fins registered");
    }

    public static TechType TechType { get; private set; }

}
