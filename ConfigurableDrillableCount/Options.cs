namespace ConfigurableDrillableCount;

using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Options;
using System;
using UnityEngine;

public class Options : ModOptions
{
    internal static void Initialize()
    {
        OptionsPanelHandler.RegisterModOptions(new Options("Configurable Drillable Count"));
    }

    public Options(string name) : base(name)
    {
        var min = ModSliderOption.Create("cdcMin", "Minimum", 0, 10, CDC_Config.Min);
        min.OnChanged += OnSliderChanged;
        var max = ModSliderOption.Create("cdcMax", "Maximum", 0, 10, CDC_Config.Max);
        max.OnChanged += OnSliderChanged;

        AddItem(min);
        AddItem(max);
    }

    public void OnSliderChanged(object sender, SliderChangedEventArgs e)
    {
        int val = (int)Math.Round(e.Value, 0);

        if (e.Id == "cdcMin")
        {
            Plugin.Logger.LogInfo($"Minimum value updated from {CDC_Config.Min} to {val}");
            CDC_Config.Min = val;
            PlayerPrefs.SetInt("cdcMin", val);
            PlayerPrefs.Save();
        }
        else if (e.Id == "cdcMax")
        {
            Plugin.Logger.LogInfo($"Maximum value updated from {CDC_Config.Max} to {val}");
            CDC_Config.Max = val;
            PlayerPrefs.SetInt("cdcMax", val);
            PlayerPrefs.Save();
        }

        UnityEngine.Object.FindObjectsOfType<CDC_Config>().Do(cdc => cdc.UpdateNumbers());
        Plugin.Logger.LogInfo("Updated Drillable components for all objects");
    }
}