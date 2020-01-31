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
            new Hatch("East", 90).Patch();
            new Hatch("South", 180).Patch();
            new Hatch("West", 270).Patch();

            Logger.Log("Patched");
        }
    }

    public class Hatch : Buildable
    {
        public int Angle;

        public Hatch(string direction, int angle) : base("rotatedhatch" + direction.ToLower(), "Rotated Hatch (" + direction + ")", "Provides an access point to the habitat. Rotated to face" + direction.ToLower())
        {
            this.Angle = angle;

            OnFinishedPatching += () =>
            {
                CraftDataHandler.RemoveFromGroup(TechGroup.BasePieces, TechCategory.BasePiece, this.TechType);
                CraftDataHandler.AddToGroup(TechGroup.BasePieces, TechCategory.BasePiece, this.TechType, TechType.BaseHatch);
            };
        }

        public override TechGroup GroupForPDA => TechGroup.BasePieces;
        public override TechCategory CategoryForPDA => TechCategory.BasePiece;

        protected override Atlas.Sprite GetItemSprite() => SpriteManager.Get(TechType.BaseHatch);
        protected override TechData GetBlueprintRecipe() => new TechData(new Ingredient(TechType.Titanium, 2), new Ingredient(TechType.Quartz, 1));

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.BaseHatch);
            GameObject obj = GameObject.Instantiate(prefab);

            obj.transform.GetChild(0).rotation = Quaternion.Euler(0, this.Angle, 0);

            return obj;
        }
    }
}
