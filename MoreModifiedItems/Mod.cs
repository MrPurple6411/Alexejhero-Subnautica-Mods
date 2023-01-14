namespace MoreModifiedItems
{
    using AlexejheroYTB.Common;
    using Harmony;
    using QModManager.API.ModLoading;
    using SMLHelper.Assets;
    using SMLHelper.Crafting;
    using SMLHelper.Handlers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using UnityEngine;

    [QModCore]
    public class Plugin
    {
        [QModPatch]
        public void Awake()
        {
            HarmonyHelper.Patch();

            new LightweightUltraHighCapacityTank().Patch();
            new UltraGlideSwimChargeFins().Patch();
            new ReinforcedStillsuit().Patch();

            AlexejheroYTB.Common.Logger.Log("Patched");
        }
    }

    public class LightweightUltraHighCapacityTank : Craftable
    {
        public LightweightUltraHighCapacityTank() : base("lwuhtank", "Lightweight Ultra High Capacity Tank", "Has the same amount of oxygen as the Ultra High Capacity Tank, but has the no speed penalty bonus of the Lightweight High Capacity Tank.")
        {
            base.OnFinishedPatching += OnFinishedPatching;
        }

        public new void OnFinishedPatching()
        {
            CraftDataHandler.RemoveFromGroup(TechGroup.Workbench, TechCategory.Workbench, this.TechType);
            CraftDataHandler.AddToGroup(TechGroup.Workbench, TechCategory.Workbench, this.TechType, TechType.HighCapacityTank);

            SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(TechType.HighCapacityTank));

            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.Tank);
            CraftDataHandler.SetItemSize(this.TechType, 3, 4);
            CraftDataHandler.SetCraftingTime(this.TechType, 5);
        }

        public override TechGroup GroupForPDA => TechGroup.Workbench;
        public override TechCategory CategoryForPDA => TechCategory.Workbench;

        protected override TechData GetBlueprintRecipe() => new TechData(
            new Ingredient(TechType.HighCapacityTank, 1),
            new Ingredient(TechType.PlasteelTank, 1),
            new Ingredient(TechType.Lubricant, 2),
            new Ingredient(TechType.HydrochloricAcid, 1)
        )
        {
            craftAmount = 1,
        };
        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
        public override string[] StepsToFabricatorTab => "TankMenu".Split('/');

        public override GameObject GetGameObject()
        {
            GameObject obj = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.PlasteelTank));

            obj.GetAllComponentsInChildren<Oxygen>().Do(o => o.oxygenCapacity = 180);

            return obj;
        }
    }

    public class UltraGlideSwimChargeFins : Craftable
    {
        public static TechType techType;

        public UltraGlideSwimChargeFins() : base("ugscfins", "Ultra Glide Swim Charge Fins", "Has the same speed increase as the Ultra Glide Fins, but also has the tool recharge ability of the Swim Charge Fins.")
        {
            base.OnFinishedPatching += OnFinishedPatching;
        }

        public new void OnFinishedPatching()
        {
            CraftDataHandler.RemoveFromGroup(TechGroup.Workbench, TechCategory.Workbench, this.TechType);
            CraftDataHandler.AddToGroup(TechGroup.Workbench, TechCategory.Workbench, this.TechType, TechType.SwimChargeFins);

            SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(TechType.SwimChargeFins));

            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.Foots);
            CraftDataHandler.SetItemSize(this.TechType, 2, 3);
            CraftDataHandler.SetCraftingTime(this.TechType, 5);

            techType = TechType;
        }

        public override TechGroup GroupForPDA => TechGroup.Workbench;
        public override TechCategory CategoryForPDA => TechCategory.Workbench;

        protected override TechData GetBlueprintRecipe() => new TechData(
            new Ingredient(TechType.UltraGlideFins, 1),
            new Ingredient(TechType.SwimChargeFins, 1),
            new Ingredient(TechType.Lubricant, 2),
            new Ingredient(TechType.HydrochloricAcid, 1)
        )
        {
            craftAmount = 1,
        };
        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
        public override string[] StepsToFabricatorTab => "FinsMenu".Split('/');

        public override GameObject GetGameObject()
        {
            return GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.UltraGlideFins));
        }

        [HarmonyPatch(typeof(UpdateSwimCharge), nameof(UpdateSwimCharge.FixedUpdate))]
        public static class UpdateSwimCharge_FixedUpdate_Patch
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> c = instructions.ToList();
                int index = c.FindIndex(o => o.opcode == OpCodes.Ldc_I4_0);

                c.InsertRange(index, new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Inventory), nameof(Inventory.Get))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(Inventory), nameof(Inventory.equipment)).GetGetMethod()),
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(UltraGlideSwimChargeFins), nameof(UltraGlideSwimChargeFins.techType))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Equipment), nameof(Equipment.GetCount))),
                new CodeInstruction(OpCodes.Add),
            });

                return c;
            }
        }

        [HarmonyPatch(typeof(UnderwaterMotor), nameof(UnderwaterMotor.AlterMaxSpeed))]
        public static class UnderwaterMotor_AlterMaxSpeed_Patch
        {
            public static void Postfix(UnderwaterMotor __instance, ref float __result)
            {
                if (Inventory.Get().equipment.GetCount(techType) > 0)
                {
                    __result += 2.5f * __instance.currentWreckSpeedMultiplier;
                }
            }
        }
    }

    public class ReinforcedStillsuit : Craftable
    {
        public static TechType techType;

        public ReinforcedStillsuit() : base("rssuit", "Reinforced Stillsuit", "Offers the same protection as the Reinforced Dive Suit, and also has the water recycling feature of the Stillsuit")
        {
            base.OnStartedPatching += OnStartedPatching;
            base.OnFinishedPatching += OnFinishedPatching;
        }

        public new void OnStartedPatching()
        {
            CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "BodyMenu", "Suit Upgrades", SpriteManager.Get(TechType.Stillsuit));
        }

        public new void OnFinishedPatching()
        {
            CraftDataHandler.RemoveFromGroup(TechGroup.Workbench, TechCategory.Workbench, this.TechType);
            CraftDataHandler.AddToGroup(TechGroup.Workbench, TechCategory.Workbench, this.TechType, TechType.Stillsuit);

            SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(TechType.Stillsuit));

            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.Body);
            CraftDataHandler.SetItemSize(this.TechType, 2, 3);
            CraftDataHandler.SetCraftingTime(this.TechType, 5);

            techType = TechType;
        }

        public override TechGroup GroupForPDA => TechGroup.Workbench;
        public override TechCategory CategoryForPDA => TechCategory.Workbench;

        protected override TechData GetBlueprintRecipe() => new TechData(
            new Ingredient(TechType.ReinforcedDiveSuit, 1),
            new Ingredient(TechType.Stillsuit, 1),
            new Ingredient(TechType.Lubricant, 2),
            new Ingredient(TechType.HydrochloricAcid, 1)
        )
        {
            craftAmount = 1,
        };
        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
        public override string[] StepsToFabricatorTab => "BodyMenu".Split('/');

        public override GameObject GetGameObject()
        {
            return GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.Stillsuit));
        }

        [HarmonyPatch(typeof(Player), nameof(Player.HasReinforcedSuit))]
        public static class Player_HasReinforcedSuit_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(ref bool __result)
            {
                __result = __result || Inventory.main.equipment.GetTechTypeInSlot("Body") == techType;
            }
        }
    }
}
