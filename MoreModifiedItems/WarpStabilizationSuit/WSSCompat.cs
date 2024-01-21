namespace MoreModifiedItems.WarpStabilizationSuit;

using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

internal static class WSSCompat
{
    internal static void Patch()
    {
        Type ModdedSuitsManager = AccessTools.TypeByName("SuitLib.ModdedSuitsManager");
        if (ModdedSuitsManager == null)
        {
            Plugin.Log.LogDebug("ModdedSuitsManager not found, skipping WSSCompat patch");
            return;
        }

        FieldInfo moddedSuitsList = AccessTools.Field(ModdedSuitsManager, "moddedSuitsList");
        if (moddedSuitsList == null)
        {
            Plugin.Log.LogError("ModdedSuitsManager.moddedSuitsList not found, skipping WSSCompat patch");
            return;
        }

        MethodInfo AddModdedSuit = AccessTools.Method(ModdedSuitsManager, "AddModdedSuit");

        if (AddModdedSuit == null)
        {
            Plugin.Log.LogError("ModdedSuitsManager.AddModdedSuit not found, skipping WSSCompat patch");
            return;
        }

        Type ModdedSuit = AccessTools.TypeByName("SuitLib.ModdedSuit");
        if (ModdedSuit == null)
        {
            Plugin.Log.LogError("ModdedSuit not found, skipping WSSCompat patch");
            return;
        }

        ConstructorInfo constructor = AccessTools.FirstConstructor(ModdedSuit, (c) =>
        {
            if (c.GetParameters().Length == 7)
                return true;

            return false;
        });

        if (constructor == null)
        {
            Plugin.Log.LogError("ModdedSuit constructor not found, skipping WSSCompat patch");
            return;
        }

        FieldInfo itemTechType = AccessTools.Field(ModdedSuit, "itemTechType");

        if (itemTechType == null)
        {
            Plugin.Log.LogError("ModdedSuit.itemTechType not found, skipping WSSCompat patch");
            return;
        }

        List<FieldInfo> fieldInfos = AccessTools.GetDeclaredFields(ModdedSuit);

        if (fieldInfos == null)
        {
            Plugin.Log.LogError("ModdedSuit fieldInfos not found, skipping WSSCompat patch");
            return;
        }

        var moddedSuits = (IList)moddedSuitsList.GetValue(null);

        if (moddedSuits == null)
        {
            Plugin.Log.LogError("ModdedSuitsManager.moddedSuitsList not found, skipping WSSCompat patch");
            return;
        }

        object WarpStabilizationSuit = null;

        foreach (var suit in moddedSuits)
        {
            var techType = itemTechType.GetValue(suit);

            if (techType.ToString() == "WarpStabilizationSuit")
            {
                WarpStabilizationSuit = suit;
                break;
            }
        }

        if (WarpStabilizationSuit == null)
        {
            Plugin.Log.LogError("WarpStabilizationSuit not found, skipping WSSCompat patch");
            return;
        }
        
        StabilizedEnhancedStillsuit.CreateAndRegister();

        var StabilizedEnhancedStillsuitModdedSuit = constructor.Invoke(new object[7]);
                
        foreach (var fieldInfo in fieldInfos)
        {
            if (fieldInfo.FieldType == typeof(TechType))
            {
                Plugin.Log.LogDebug("Setting StabilizedEnhancedStillsuitModdedSuit itemTechType to " + StabilizedEnhancedStillsuit.Instance.Info.TechType);
                fieldInfo.SetValue(StabilizedEnhancedStillsuitModdedSuit, StabilizedEnhancedStillsuit.Instance.Info.TechType);
                continue;
            }
            fieldInfo.SetValue(StabilizedEnhancedStillsuitModdedSuit, fieldInfo.GetValue(WarpStabilizationSuit));
        }

        AddModdedSuit.Invoke(null, new object[] { StabilizedEnhancedStillsuitModdedSuit });

        StabilizedEnhancedStillsuitMK2.CreateAndRegister();

        object StabilizedEnhancedStillsuitMK2ModdedSuit = constructor.Invoke(new object[7]);

        foreach (var fieldInfo in fieldInfos)
        {
            if (fieldInfo.FieldType == typeof(TechType))
            {
                Plugin.Log.LogDebug("Setting StabilizedEnhancedStillsuitMK2ModdedSuit itemTechType to " + StabilizedEnhancedStillsuitMK2.Instance.Info.TechType);
                fieldInfo.SetValue(StabilizedEnhancedStillsuitMK2ModdedSuit, StabilizedEnhancedStillsuitMK2.Instance.Info.TechType);
                continue;
            }
            fieldInfo.SetValue(StabilizedEnhancedStillsuitMK2ModdedSuit, fieldInfo.GetValue(WarpStabilizationSuit));
        }

        AddModdedSuit.Invoke(null, new object[] { StabilizedEnhancedStillsuitMK2ModdedSuit });

        StabilizedEnhancedStillsuitMK3.CreateAndRegister();

        object StabilizedEnhancedStillsuitMK3ModdedSuit = constructor.Invoke(new object[7]);

        foreach (var fieldInfo in fieldInfos)
        {
            if (fieldInfo.FieldType == typeof(TechType))
            {
                Plugin.Log.LogDebug("Setting StabilizedEnhancedStillsuitMK3ModdedSuit itemTechType to " + StabilizedEnhancedStillsuitMK3.Instance.Info.TechType);
                fieldInfo.SetValue(StabilizedEnhancedStillsuitMK3ModdedSuit, StabilizedEnhancedStillsuitMK3.Instance.Info.TechType);
                continue;
            }
            fieldInfo.SetValue(StabilizedEnhancedStillsuitMK3ModdedSuit, fieldInfo.GetValue(WarpStabilizationSuit));
        }

        AddModdedSuit.Invoke(null, new object[] { StabilizedEnhancedStillsuitMK3ModdedSuit });
    }
}
