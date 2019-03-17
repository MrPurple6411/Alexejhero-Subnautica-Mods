using AlexejheroYTB.Common;
using MP3player;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AlexejheroYTB.SonyWalkman
{
    public static class Mod
    {
        public static readonly string assetsPath = Path.Combine(new DirectoryInfo(Path.Combine(Assembly.GetExecutingAssembly().Location, "..")).Name, "Assets");

        public static void Patch()
        {
            HarmonyHelper.Patch();
            new SonyWalkman().Patch();
        }
    }

    public static class Language
    {
        public const string id = "sonywalkman";
        public const string displayName = "Sony Walkman";
        public const string tooltip = "A music player your grandpa gave to you when you where a child.\nCan play music from the \"OST\" folder.\nI recommend Abandon Ship.";
        public const string leftClickTooltip = "play / pause";
        public const string middleClickTooltip = "change song";
    }

    public class SonyWalkman : Craftable
    {
        public new void Patch()
        {
            base.Patch();
            ItemActionHelper.RegisterAction(MouseButton.Left, TechType, OnLeftClick, Language.leftClickTooltip, true.ToPredicate<InventoryItem>());
            ItemActionHelper.RegisterAction(MouseButton.Middle, TechType, OnMiddleClick, Language.middleClickTooltip, true.ToPredicate<InventoryItem>());
        }

        public SonyWalkman() : base(Language.id, Language.displayName, Language.tooltip) { }

        protected override TechData GetBlueprintRecipe() => new TechData() { craftAmount = 1 };
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => "Personal/Equipment".Split('/');

        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Equipment;

        public override string AssetsFolder => Mod.assetsPath;
        public override string IconFileName => "radio.png";

        public override GameObject GetGameObject()
        {
            GameObject prefab = Resources.Load<GameObject>("worldentities/tools/battery");
            GameObject obj = Object.Instantiate(prefab);

            Pickupable pickupable = obj.GetComponent<Pickupable>();
            pickupable.destroyOnDeath = false;

            Object.DestroyImmediate(obj.GetComponent<Battery>());

            OSTAudioPlayer radio = obj.AddComponent<OSTAudioPlayer>();

            return obj;
        }

        public void OnLeftClick(InventoryItem item) => item.item.gameObject.GetComponent<OSTAudioPlayer>().OnLeftClick();
        public void OnMiddleClick(InventoryItem item) => item.item.gameObject.GetComponent<OSTAudioPlayer>().OnMiddleClick();
    }

    public class OSTAudioPlayer : MonoBehaviour
    {
        public bool playing = false;
        public string[] songs;
        public int songIndex;

        public MethodInfo SetMusicVolume;
        public FieldInfo musicVolume;
        public float musicVolumeBackup;

        public MusicPlayer currentPlayer;

        public void Awake()
        {
            songs = Directory.GetFiles(Path.Combine(Application.dataPath, "../OST"), "*.mp3").Select(file => new FileInfo(file).Name).ToArray();
            songIndex = 0;
            SetMusicVolume = typeof(GameInput).Assembly.GetType("SoundSystem").GetMethod("SetMusicVolume", BindingFlags.Static | BindingFlags.Public);
            musicVolume = typeof(GameInput).Assembly.GetType("SoundSystem").GetField("musicVolume", BindingFlags.Static | BindingFlags.NonPublic);
            musicVolumeBackup = (float)musicVolume.GetValue(null);
            currentPlayer = new MusicPlayer();
        }

        public void OnLeftClick()
        {
            if (playing)
            {
                currentPlayer.Pause();
                SetMusicVolume.Invoke(null, new object[] { musicVolumeBackup });
                ErrorMessage.AddMessage("Paused");
            }
            else
            {
                currentPlayer.Play(false);
                musicVolumeBackup = (float)musicVolume.GetValue(null);
                SetMusicVolume.Invoke(null, new object[] { 0f });
                musicVolume.SetValue(null, musicVolumeBackup);
                ErrorMessage.AddMessage("Unpaused");
            }

            playing = !playing;
        }
        public void OnMiddleClick()
        {
            ErrorMessage.AddMessage($"Changing song...");
            songIndex++;
            currentPlayer.Pause();
            currentPlayer.Close();

            string audioName = songs[songIndex];
            currentPlayer.Open(Path.Combine(Application.dataPath, $"../OST/{audioName}"));

            ErrorMessage.AddMessage($"Now playing: {audioName.Substring(0, audioName.Length - 4)}");
            currentPlayer.Play(false);
        }
    }
}
