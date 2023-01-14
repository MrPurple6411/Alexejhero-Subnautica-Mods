namespace AlexejheroYTB.HorizontalWallLockers
{
    using AlexejheroYTB.Common;
    using QModManager.API.ModLoading;
    using SMLHelper.Assets;
    using SMLHelper.Crafting;
    using SMLHelper.Handlers;
    using UnityEngine;
    using Logger = AlexejheroYTB.Common.Logger;

    [QModCore]
    public class Plugin
    {
        [QModPatch]
        public void Awake()
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
