namespace PickupableStorageEnhanced;
using Nautilus.Options;

public class Options : ModOptions
{
    public Options(string name) : base(name)
    {
        var enabled = ModToggleOption.Create("pfcEnable", "Enable", PFC_Config.Enable);
        enabled.OnChanged += OnToggleChanged;
        AddItem(enabled);

        var MMB = ModChoiceOption<string>.Create("pfcMMB", "Open storage in inventory", PFC_Config.AllowMMBOptions, PFC_Config.AllowMMB);
        MMB.OnChanged += OnChoiceChanged;
        AddItem(MMB);
    }

    public void OnToggleChanged(object sender, ToggleChangedEventArgs e)
    {
        if (e.Value) Plugin.Logger.LogInfo("Enabled mod");
        else Plugin.Logger.LogInfo("Disabled mod");
        PFC_Config.Enable = e.Value;
    }

    public void OnChoiceChanged(object sender, ChoiceChangedEventArgs<string> e)
    {
        Plugin.Logger.LogInfo($"Set storage opening in inventory to: \"{e.Value}\"");
        PFC_Config.AllowMMB = e.Value;
    }
}