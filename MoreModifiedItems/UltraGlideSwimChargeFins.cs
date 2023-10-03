namespace MoreModifiedItems;

using HarmonyLib;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Crafting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using static CraftData;
using Nautilus.Assets.Gadgets;

[HarmonyPatch]
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

        var cg = Instance.SetRecipe(new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>()
            {
                new Ingredient(TechType.UltraGlideFins, 1),
                new Ingredient(TechType.SwimChargeFins, 1),
                new Ingredient(TechType.Lubricant, 2),
                new Ingredient(TechType.HydrochloricAcid, 1)
            }
        }).WithCraftingTime(5f).WithFabricatorType(CraftTree.Type.Workbench);

        if (Plugin.OrganizedWorkbench)
            cg.WithStepsToFabricatorTab("FinsMenu".Split('/'));

        if (GetBuilderIndex(TechType.UltraGlideFins, out var group, out var category, out _))
            Instance.SetPdaGroupCategoryAfter(group, category, TechType.UltraGlideFins);

        var cloneStillsuit = new CloneTemplate(Instance.Info, TechType.UltraGlideFins)
        {
            ModifyPrefab = (obj) => { }
        };

        Instance.SetGameObject(cloneStillsuit);

        Instance.Register();

        techType = Instance.Info.TechType;
    }

    private static TechType techType;

    [HarmonyPatch(typeof(UpdateSwimCharge), nameof(UpdateSwimCharge.FixedUpdate))]
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

    [HarmonyPatch(typeof(UnderwaterMotor), nameof(UnderwaterMotor.AlterMaxSpeed))]
    [HarmonyPostfix]
    public static void UnderwaterMotor_AlterMaxSpeed_Postfix(UnderwaterMotor __instance, ref float __result)
    {
        if (Inventory.Get().equipment.GetCount(techType) > 0)
        {
            __result += 2.5f * __instance.currentPlayerSpeedMultipler;
        }
    }
}
