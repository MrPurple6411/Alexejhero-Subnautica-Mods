namespace MoreModifiedItems.DeathrunRemade;

using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

internal static class DeathrunCompat
{
    private static bool _deathrunCheck = false;
    private static bool _deathrunVersionCheck = false;
    private static bool _deathrunCheckFailed = false;
    private static FieldInfo _nextUpdate;
    private static FieldInfo _equipment;
    private static MethodInfo UpdatePhotoSynthesisTank;
    private static MethodInfo UpdateChemosynthesisTank;

    private static PluginInfo DeathrunPlugin { get; set; }
    private static Type DeathrunAPI { get; set; }
    private static MethodInfo AddSuitCrushDepth { get; set; }
    private static MethodInfo AddNitrogenModifier { get; set; }
    public static Type DeathrunTank { get; private set; }

    public static bool DeathrunLoaded()
    {
        if (_deathrunCheck)
        {
            return DeathrunPlugin != null;
        }

        _deathrunCheck = true;
        if (!Chainloader.PluginInfos.TryGetValue("com.github.tinyhoot.DeathrunRemade", out PluginInfo plugin))
        {
            return false;
        }

        DeathrunPlugin = plugin;
        return true;
    }

    public static bool VersionCheck()
    {
        if (_deathrunVersionCheck)
        {
            return !_deathrunCheckFailed;
        }

        _deathrunVersionCheck = true;

        if (!DeathrunLoaded())
        {
            Plugin.Log.LogDebug("Deathrun not loaded");
            _deathrunCheckFailed = true;
            return false;
        }

        if (DeathrunPlugin.Metadata.Version < Version.Parse("0.1.5"))
        {
            Plugin.Log.LogWarning("Deathrun version below 0.1.5 detected. Nitrogen Modifier API was implimented in 0.1.5. Please update your Deathrun Remade");
            _deathrunCheckFailed = true;
            return false;
        }

        DeathrunAPI = AccessTools.TypeByName("DeathrunRemade.DeathrunAPI");
        if (DeathrunAPI == null)
        {
            Plugin.Log.LogWarning("DeathrunAPI not found");
            _deathrunCheckFailed = true;
            return false;
        }

        AddSuitCrushDepth = DeathrunAPI.GetMethod("AddSuitCrushDepth", new Type[] { typeof(TechType), typeof(IEnumerable<float>) });

        if (AddSuitCrushDepth == null)
        {
            Plugin.Log.LogWarning("AddSuitCrushDepth not found");
            _deathrunCheckFailed = true;
            return false;
        }

        AddNitrogenModifier = DeathrunAPI?.GetMethod("AddNitrogenModifier", new Type[] { typeof(TechType), typeof(IEnumerable<float>) });

        if (AddNitrogenModifier == null)
        {
            Plugin.Log.LogWarning("AddNitrogenModifier not found");
            _deathrunCheckFailed = true;
            return false;
        }

        return true;
    }

    public static void AddSuitCrushDepthMethod(TechType techType, IEnumerable<float> depths)
    {
        if (VersionCheck())
        {
            Plugin.Log.LogDebug($"Adding crush depths for {techType}");
            AddSuitCrushDepth.Invoke(null, new object[] { techType, depths });
        }
    }

    public static void AddNitrogenModifierMethod(TechType techType, IEnumerable<float> nitrogen)
    {
        if (VersionCheck())
        {
            Plugin.Log.LogDebug($"Adding nitrogen modifiers for {techType}");
            AddNitrogenModifier.Invoke(null, new object[] { techType, nitrogen });
        }
    }

    public static void PatchDeathrunTank()
    {
        if (!DeathrunLoaded())
            return;

        DeathrunTank = AccessTools.TypeByName("DeathrunRemade.Components.DeathrunTank");

        if (DeathrunTank == null)
        {
            Plugin.Log.LogError("Failed to load DeathrunRemade.Components.DeathrunTank");
            return;
        }

        _nextUpdate = AccessTools.Field(DeathrunTank, "_nextUpdate");

        if (_nextUpdate == null)
        {
            Plugin.Log.LogError("Failed to load DeathrunRemade.Components.DeathrunTank._nextUpdate");
            return;
        }

        _equipment = AccessTools.Field(DeathrunTank, "_equipment");

        if (_equipment == null)
        {
            Plugin.Log.LogError("Failed to load DeathrunRemade.Components.DeathrunTank._equipment");
            return;
        }

        MethodInfo Update = AccessTools.Method(DeathrunTank, "Update");

        if (Update == null)
        {
            Plugin.Log.LogError("Failed to load DeathrunRemade.Components.DeathrunTank.Update");
            return;
        }

        UpdatePhotoSynthesisTank = AccessTools.Method(DeathrunTank, "UpdatePhotoSynthesisTank");

        if (UpdatePhotoSynthesisTank == null)
        {
            Plugin.Log.LogError("Failed to load DeathrunRemade.Components.DeathrunTank.UpdatePhotoSynthesisTank");
            return;
        }

        UpdateChemosynthesisTank = AccessTools.Method(DeathrunTank, "UpdateChemosynthesisTank");

        if (UpdateChemosynthesisTank == null)
        {
            Plugin.Log.LogError("Failed to load DeathrunRemade.Components.DeathrunTank.UpdateChemosynthesisTank");
            return;
        }

        HarmonyMethod prefix = new HarmonyMethod(typeof(DeathrunCompat).GetMethod(nameof(UpdateDeathrunTank), BindingFlags.Static | BindingFlags.Public));
        Plugin.harmony.Patch(Update, prefix: prefix);
    }

    public static void UpdateDeathrunTank(object __instance)
    {
        var nextUpdate = (float)_nextUpdate.GetValue(__instance);

        if (Time.time < nextUpdate)
            return;

        Equipment equipment = (Equipment)_equipment.GetValue(__instance);

        InventoryItem itemInSlot = equipment.GetItemInSlot("Tank");

        if (itemInSlot == null)
            return;

        Pickupable item = itemInSlot.item;
        if (item == null)
            return;

        PrefabIdentifier prefabIdentifier = item.GetComponent<PrefabIdentifier>();
        if (prefabIdentifier == null)
            return;

        if (prefabIdentifier.ClassId == LightweightUltraHighCapacityPhotosynthesisTank.Instance.Info.ClassID)
        {
            UpdatePhotoSynthesisTank.Invoke(__instance, null);
            UpdatePhotoSynthesisTank.Invoke(__instance, null);
            UpdatePhotoSynthesisTank.Invoke(__instance, null);
        }
        else if (prefabIdentifier.ClassId == LightweightUltraHighCapacityChemosynthesisTank.Instance.Info.ClassID)
        {
            UpdateChemosynthesisTank.Invoke(__instance, null);
            UpdateChemosynthesisTank.Invoke(__instance, null);
            UpdateChemosynthesisTank.Invoke(__instance, null);
        }
    }
}
