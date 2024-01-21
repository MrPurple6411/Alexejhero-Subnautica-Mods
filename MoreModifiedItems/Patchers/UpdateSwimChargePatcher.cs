namespace MoreModifiedItems.Patchers;

using HarmonyLib;
using MoreModifiedItems.BasicEquipment;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

[HarmonyPatch(typeof(UpdateSwimCharge))]
internal static class UpdateSwimChargePatcher
{
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
            new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(UltraGlideSwimChargeFins), nameof(UltraGlideSwimChargeFins.TechType)).GetGetMethod()),
            new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Equipment), nameof(Equipment.GetCount))),
            new CodeInstruction(OpCodes.Add),
        });

        return c;
    }
}
