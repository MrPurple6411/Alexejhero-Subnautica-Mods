using AlexejheroYTB.Common;
using MP3player;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AlexejheroYTB.Radio
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
        public const string id = "mp3playerradio";
        public const string displayName = "MP3 Player";
        public const string tooltip = "Can play music from the \"OST\" folder.\nI recommend Abandon Ship.";
        public const string leftClickTooltip = "play/pause";
        public const string middleClickTooltip = "open/close interface";
    }

    public class SonyWalkman : Craftable
    {
        public new void Patch()
        {
            base.Patch();
            ItemActionHelper.RegisterAction(MouseButton.Left, TechType, OnLeftClick, Language.leftClickTooltip, (item) => !item.item.gameObject.GetComponent<OSTAudioPlayer>().IsOpen);
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

            PseudoStorage storage = obj.AddComponent<PseudoStorage>();

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
        public bool IsOpen = false;

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
            IsOpen = !IsOpen;

            if (!IsOpen)
            {
                Player.main.GetPDA().Close();
                Player.main.GetPDA().Open(PDATab.Inventory);
                return;
            }

            Player.main.GetPDA().Close();

            StorageContainer container = GetComponentInChildren<StorageContainer>();
            container.Open();
            container.onUse.Invoke();

            /*
            ErrorMessage.AddMessage($"Changing song...");
            songIndex++;
            currentPlayer.Pause();
            currentPlayer.Close();

            string audioName = songs[songIndex];
            currentPlayer.Open(Path.Combine(Application.dataPath, $"../OST/{audioName}"));

            ErrorMessage.AddMessage($"Now playing: {audioName.Substring(0, audioName.Length - 4)}");
            currentPlayer.Play(false);
            */
        }
    }

    public class PseudoStorage : StorageContainer
    {
        public override void Awake()
        {
            GameObject pseudoStorage = new GameObject("PseudoStorage");
            pseudoStorage.transform.SetParent(transform);

            height = 4;
            width = 6;
            storageLabel = "MP3 PLAYER";
            storageRoot = pseudoStorage.AddComponent<ChildObjectIdentifier>();

            base.Awake();

            container.isAllowedToAdd = new IsAllowedToAdd((p, b) => false);
            container.isAllowedToRemove = new IsAllowedToRemove((p, b) => false);
        }
    }
}
