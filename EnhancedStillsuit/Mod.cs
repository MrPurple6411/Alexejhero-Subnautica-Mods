using AlexejheroYTB.Common;
using Harmony;
using QModManager.API;
using QModManager.API.ModLoading;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using System.Collections.Generic;
using UnityEngine;

namespace AlexejheroYTB.EnhancedStillsuit
{
    [QModCore]
    public static class Mod
    {
        [QModPatch]
        public static void Patch()
        {
            HarmonyHelper.Patch();
            new ESS().Patch();

            Common.Logger.Log("Patched");
        }
    }

    public class ESS : Craftable
    {
        public ESS() : base("enhancedstillsuit", "Enhanced Stillsuit", "Just like a normal stillsuit, but it automatically drinks the generated reclaimed water.")
        {
            base.OnStartedPatching += OnStartedPatching;
            base.OnFinishedPatching += OnFinishedPatching;
        }

        public new void OnStartedPatching()
        {
            if (!QModServices.Main.ModPresent("MoreModifiedItems"))
            {
                CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "BodyMenu", "Suit Upgrades", SpriteManager.Get(TechType.Stillsuit));
            }
        }

        public new void OnFinishedPatching()
        {
            CraftDataHandler.SetItemSize(this.TechType, 2, 2);
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.Body);
            SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(TechType.Stillsuit));
        }

        protected override TechData GetBlueprintRecipe() => new TechData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>()
            {
                new Ingredient(TechType.Stillsuit, 1),
                new Ingredient(TechType.ComputerChip, 1),
                new Ingredient(TechType.CopperWire, 2),
                new Ingredient(TechType.Silver, 1),
            },
        };
        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
        public override string[] StepsToFabricatorTab => new string[] { "BodyMenu" };

        public override TechCategory CategoryForPDA => TechCategory.Equipment;
        public override TechGroup GroupForPDA => TechGroup.Personal;

        public override TechType RequiredForUnlock => TechType.Stillsuit;

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.Stillsuit);
            GameObject obj = GameObject.Instantiate(prefab);

            obj.AddComponent<ESSBehaviour>();

            return obj;
        }
    }

    public class ESSBehaviour : MonoBehaviour { }

    [HarmonyPatch(typeof(Stillsuit), nameof(Stillsuit.UpdateEquipped))]
    [HarmonyPatch("UpdateEquipped")]
    public static class StillSuit_UpdateEquipped
    {
        [HarmonyPrefix]
        public static bool Prefix(Stillsuit __instance)
        {
            if (!__instance.GetComponent<ESSBehaviour>()) return true;

            if (GameModeUtils.RequiresSurvival() && !Player.main.GetComponent<Survival>().freezeStats)
            {
                ErrorMessage.AddDebug(Mathf.RoundToInt(__instance.waterCaptured).ToString() + "/" + __instance.waterPrefab.waterValue.ToString());


                __instance.waterCaptured += Time.deltaTime / 18f * 0.75f;
                if (__instance.waterCaptured >= __instance.waterPrefab.waterValue)
                {
                    ErrorMessage.AddDebug("Enhanced Stillsuit activated!");

                    GameObject gameObject = GameObject.Instantiate(__instance.waterPrefab.gameObject);
                    Pickupable component = gameObject.GetComponent<Pickupable>();
                    Player.main.GetComponent<Survival>().Eat(component.gameObject);
                    __instance.waterCaptured -= __instance.waterPrefab.waterValue;
                }
            }

            return false;
        }
    }
}