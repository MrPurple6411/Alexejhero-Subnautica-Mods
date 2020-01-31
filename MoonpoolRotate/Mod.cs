using AlexejheroYTB.Common;
using QModManager.API.ModLoading;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using UnityEngine;
using Logger = AlexejheroYTB.Common.Logger;

namespace AlexejheroYTB.DirectionalHatchControl
{
    [QModCore]
    public static class QMod
    {
        [QModPatch]
        public static void Patch()
        {
            new RotatedMoonpool().Patch();

            Logger.Log("Patched");
        }
    }

    public class RotatedMoonpool : Buildable
    {
        public RotatedMoonpool() : base("rotatedmoonpool", "Rotated Moonpool", "A rotated version of the moonpool")
        {
            OnFinishedPatching += () =>
            {
                CraftDataHandler.RemoveFromGroup(TechGroup.BasePieces, TechCategory.BasePiece, this.TechType);
                CraftDataHandler.AddToGroup(TechGroup.BasePieces, TechCategory.BasePiece, this.TechType, TechType.BaseMoonpool);
            };
        }

        public override TechGroup GroupForPDA => TechGroup.BasePieces;
        public override TechCategory CategoryForPDA => TechCategory.BasePiece;

        protected override Atlas.Sprite GetItemSprite() => SpriteManager.Get(TechType.BaseMoonpool);
        protected override TechData GetBlueprintRecipe() => new TechData(new Ingredient(TechType.TitaniumIngot, 2), new Ingredient(TechType.Lubricant, 1), new Ingredient(TechType.Lead, 2));

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.BaseMoonpool);
            GameObject obj = GameObject.Instantiate(prefab);

            obj.transform.GetChild(0).rotation = Quaternion.Euler(0, 90, 0);

            return obj;
        }
    }
}
