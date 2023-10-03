namespace InstantBulkheadAnimations;

using Nautilus.Options;
using Nautilus.Utility;
using UnityEngine;

public class Options : ModOptions
{
    public static bool Enable = true;

    public Options(string name) : base(name)
    {
        var toggle = ModToggleOption.Create("ibaEnable", "Enable", Enable);
        toggle.OnChanged += OnToggleChanged;
        AddItem(toggle);
    }

    public void OnToggleChanged(object sender, ToggleChangedEventArgs e)
    {
        if (e.Value) Plugin.Logger.LogInfo("Enabled mod");
        else Plugin.Logger.LogInfo("Disabled mod");
        Enable = e.Value;
        PlayerPrefsExtra.SetBool("ibaEnable", e.Value);
        PlayerPrefs.Save();
    }
}