using Harmony;
using QModManager.API.ModLoading;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using System.Collections.Generic;
using UnityEngine;

namespace MoreModifiedItems
{
    [QModCore]
    public static class QMod
    {
        [QModPatch]
        public static void Patch()
        {
            new LwUhTank().Patch();
        }
    }

    public class LwUhTank : Craftable
    {
        public LwUhTank() : base("lwuhtank", "Lightweight Ultra High Capacity Tank", "Has the same amount of oxygen as the Ultra High Capacity Tank, but has the no speed penalty bonus of the Lightweight High Capacity Tank.")
        {
            OnFinishedPatching += () =>
            {
                #region Crafting recipes

                CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "lwuhtanktab", "Lightweight Ultra High Capacity Tank", SpriteManager.Get(TechType.HighCapacityTank));

                TechType recipe1 = TechTypeHandler.AddTechType("lwuhtankrec1", "Modify High Capacity O2 Tank", "Use an existing High Capacity O2 Tank and modify it");
                SpriteHandler.RegisterSprite(recipe1, SpriteManager.Get(TechType.HighCapacityTank));
                CraftDataHandler.SetTechData(recipe1, new TechData(
                    new Ingredient(TechType.DoubleTank, 1),
                    new Ingredient(TechType.Lithium, 4),
                    new Ingredient(TechType.PlasteelIngot, 1),
                    new Ingredient(TechType.Lubricant, 2)
                )
                {
                    craftAmount = 0,
                    LinkedItems = new List<TechType>() { this.TechType },
                });
                CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, recipe1, "lwuhtanktab");

                TechType recipe2 = TechTypeHandler.AddTechType("lwuhtankrec2", "Modify Ultra High Capacity Tank", "Use an existing Ultra High Capacity Tank and modify it");
                SpriteHandler.RegisterSprite(recipe2, SpriteManager.Get(TechType.HighCapacityTank));
                CraftDataHandler.SetTechData(recipe2, new TechData(
                    new Ingredient(TechType.HighCapacityTank, 1),
                    new Ingredient(TechType.PlasteelIngot, 1),
                    new Ingredient(TechType.Lubricant, 2)
                )
                {
                    craftAmount = 0,
                    LinkedItems = new List<TechType>() { this.TechType },
                });
                CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, recipe2, "lwuhtanktab");

                TechType recipe3 = TechTypeHandler.AddTechType("lwuhtankrec3", "Modify Lightweight High Capacity Tank", "Use an existing Lightweight High Capacity Tank and modify it");
                SpriteHandler.RegisterSprite(recipe3, SpriteManager.Get(TechType.HighCapacityTank));
                CraftDataHandler.SetTechData(recipe3, new TechData(
                    new Ingredient(TechType.PlasteelTank, 1),
                    new Ingredient(TechType.Lithium, 4),
                    new Ingredient(TechType.Lubricant, 2)
                )
                {
                    craftAmount = 0,
                    LinkedItems = new List<TechType>() { this.TechType },
                });
                CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, recipe3, "lwuhtanktab");

                #endregion

                CraftDataHandler.RemoveFromGroup(TechGroup.Workbench, TechCategory.Workbench, this.TechType);
                CraftDataHandler.AddToGroup(TechGroup.Workbench, TechCategory.Workbench, this.TechType, TechType.HighCapacityTank);

                SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(TechType.HighCapacityTank));

                CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.Tank);
                CraftDataHandler.SetItemSize(this.TechType, 3, 4);
            };
        }

        public override TechGroup GroupForPDA => TechGroup.Workbench;
        public override TechCategory CategoryForPDA => TechCategory.Workbench;

        protected override TechData GetBlueprintRecipe() => new TechData(
            new Ingredient(TechType.DoubleTank, 1),
            new Ingredient(TechType.Lithium, 4),
            new Ingredient(TechType.PlasteelIngot, 1),
            new Ingredient(TechType.Lubricant, 2)
        );

        public override GameObject GetGameObject()
        {
            GameObject obj = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.PlasteelTank));

            obj.GetAllComponentsInChildren<Oxygen>().Do(o => o.oxygenCapacity = 180);

            return obj;
        }
    }
}
