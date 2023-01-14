namespace NoMenuPause
{
    using SMLHelper.Handlers;
    using SMLHelper.Options;

    public class Options: ModOptions
    {
        public Options() : base("No Menu Pause")
        {
            var toggle = ModToggleOption.Create("NPM", "Pause while menu is open", Plugin.NMP);
            toggle.OnChanged += (_, e) => Plugin.NMP = e.Value;
            AddItem(toggle);
        }

        internal static void Initialize()
        {
            OptionsPanelHandler.RegisterModOptions(new Options());
        }
    }

}