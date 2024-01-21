namespace MoreModifiedItems.Patchers;

using HarmonyLib;
using MoreModifiedItems.BasicEquipment;

internal static class UnderwaterMotorPatcher
{

    private static bool wasSeaglideMode = false;

    [HarmonyPatch(typeof(UnderwaterMotor), nameof(UnderwaterMotor.AlterMaxSpeed))]
    [HarmonyPrefix]
    public static void UnderwaterMotor_AlterMaxSpeed_Prefix()
    {
        if (Player.main.motorMode == Player.MotorMode.Seaglide)
        {
            Player.main.motorMode = Player.MotorMode.Dive;
            wasSeaglideMode = true;
        }
        else
        {
            wasSeaglideMode = false;
        }
    }

    [HarmonyPatch(typeof(UnderwaterMotor), nameof(UnderwaterMotor.AlterMaxSpeed))]
    [HarmonyPostfix]
    public static void UnderwaterMotor_AlterMaxSpeed_Postfix(UnderwaterMotor __instance, ref float __result)
    {
        if (Inventory.Get().equipment.GetCount(UltraGlideSwimChargeFins.TechType) > 0)
            __result += 3.2f * __instance.currentPlayerSpeedMultipler;

        if (wasSeaglideMode)
        {
            Player.main.motorMode = Player.MotorMode.Seaglide;
        }
    }
}
