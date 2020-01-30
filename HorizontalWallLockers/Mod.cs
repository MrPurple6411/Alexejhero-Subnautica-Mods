using AlexejheroYTB.Common;
using QModManager.API.ModLoading;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using UnityEngine;
using Logger = AlexejheroYTB.Common.Logger;

namespace AlexejheroYTB.HorizontalWallLockers
{
    [QModCore]
    public static class QMod
    {
        [QModPatch]
        public static void Patch()
        {
            HarmonyHelper.Patch();
            new HorizontalWallLocker().Patch();

            Logger.Log("Patched");
        }
    }

    public class HorizontalWallLocker : Buildable
    {
        public HorizontalWallLocker() : base("horizontalwalllocker", "Horizontal Wall Locker", "Small, wall-mounted storage solution.")
        {
            OnFinishedPatching += () =>
            {
                CraftDataHandler.RemoveFromGroup(TechGroup.InteriorModules, TechCategory.InteriorModule, this.TechType);
                CraftDataHandler.AddToGroup(TechGroup.InteriorModules, TechCategory.InteriorModule, this.TechType, TechType.SmallLocker);
            };
        }

        public override TechGroup GroupForPDA => TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA => TechCategory.InteriorModule;

        protected override Atlas.Sprite GetItemSprite() => SpriteManager.Get(TechType.SmallLocker);
        protected override TechData GetBlueprintRecipe() => new TechData(new Ingredient(TechType.Titanium, 2));

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.SmallLocker);
            GameObject obj = GameObject.Instantiate(prefab);

            obj.FindChild("model").transform.rotation = Quaternion.Euler(0, 0, 90);

            return obj;
        }
    }
}
