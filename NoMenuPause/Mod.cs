namespace NoMenuPause
{
    using BepInEx;
    using BepInEx.Configuration;
    using static BepInEx.Bootstrap.Chainloader;
    using HarmonyLib;
    using UWE;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "No Menu Pause",
            AUTHORS = "AlexejheroYTB, MrPurple6411",
            GUID = "com.",
            VERSION = "1.0.0.0";

        internal static ConfigEntry<bool> _nmp;

        public static bool NMP
        {
            get => !_nmp.Value;
            set => _nmp.Value = value;
        }
        #endregion

        public void Awake()
        {
            _nmp = Config.Bind(GUID, "NMP", false, "Pause while menu is open");
            var harmony = new Harmony(GUID);
            harmony.Patch(AccessTools.Method(typeof(FreezeTime), nameof(FreezeTime.Begin)), prefix: new HarmonyMethod(typeof(Plugin), nameof(Prefix)));

            if(PluginInfos.ContainsKey("com.ahk1221.smlhelper"))
            {
                Logger.LogInfo("SMLHelper Found. Initializing In-game Options Menu.");
                AccessTools.Method("NoMenuPause.Options:Initialize")?.Invoke(null, null);
            }
        }

        public static bool Prefix(FreezeTime.Id id)
        {
            if(id == FreezeTime.Id.IngameMenu && NMP)
                return false;
            return true;
        }
    }
}