namespace MoreModifiedItems.DeathrunRemade;

using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

internal static class DeathrunCompat
{
    private static bool _deathrunCheck = false;
    private static bool _deathrunVersionCheck = false;
    private static bool _deathrunCheckFailed = true;

    private static PluginInfo DeathrunPlugin { get; set; }
    private static Type DeathrunAPI { get; set; }
    private static MethodInfo AddSuitCrushDepth { get; set; }
    private static MethodInfo AddNitrogenModifier { get; set; }

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

        if (DeathrunPlugin == null)
        {
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
}
