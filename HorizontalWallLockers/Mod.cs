using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using QModManager.API.ModLoading;
using UnityEngine;
using Logger = AlexejheroYTB.Common.Logger;
using System;

namespace AlexejheroYTB.HorizontalWallLockers
{
    [QModCore]
    public static class QMod
    {
        [QModPatch]
        public static void Patch()
        {
            new HorizontalWallLocker().Patch();
            Logger.Log("Patched");
        }
    }

    public class HorizontalWallLocker : Buildable
    {
        public HorizontalWallLocker() : base("horizontalwalllocker", "Horizontal Wall Locker", "5×6 wall-mounted storage solution.") { }

        public override TechGroup GroupForPDA => TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA => TechCategory.InteriorModule;

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.SmallLocker);
            prefab.transform.Rotate(180, 180, 180);

            GameObject obj = GameObject.Instantiate(prefab);
            return obj;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData(new Ingredient(TechType.Titanium, 2));
        }
    }
}
