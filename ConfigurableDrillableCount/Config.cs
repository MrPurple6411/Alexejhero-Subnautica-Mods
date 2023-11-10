namespace ConfigurableDrillableCount;

using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

[Menu(MyPluginInfo.PLUGIN_NAME, LoadOn = MenuAttribute.LoadEvents.MenuOpened, SaveOn = MenuAttribute.SaveEvents.ChangeValue|MenuAttribute.SaveEvents.SaveGame)]
public class Config : ConfigFile
{
    [Slider("Minimum", 1, 100, Step = 1, Order = 1)]
    public int min = 1;

    [Slider("Maximum", 1, 100, Step = 1, Order = 0)]
    public int max = 3;

    public static Config Instance { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

    public static int Min => Instance.min;
    public static int Max => Instance.max;
}