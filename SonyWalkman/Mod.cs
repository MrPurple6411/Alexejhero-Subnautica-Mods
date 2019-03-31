using AlexejheroYTB.Common;
using Harmony;
using MP3player;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using System.Collections.Generic;
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
            new Radio().Patch();
            ControlItems.Patch();
        }
    }

    public class Radio : Craftable
    {
        public new void Patch()
        {
            base.Patch();
            ItemActionHelper.RegisterAction(MouseButton.Left, TechType, OnLeftClick, "play/pause", (item) => !item.item.gameObject.GetComponent<OSTAudioPlayer>().IsOpen);
            ItemActionHelper.RegisterAction(MouseButton.Middle, TechType, OnMiddleClick, "open/close interface", true.ToPredicate<InventoryItem>());
        }

        public Radio() : base("MP3Player_Radio", "MP3 Player", "Can play music from the \"OST\" folder.\nI recommend Abandon Ship.") { }

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

            OSTAudioPlayer player = obj.AddComponent<OSTAudioPlayer>();

            obj.AddComponent<PseudoStorage>().player = player;

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
        public OSTAudioPlayer player;

        public override void Awake()
        {
            GameObject pseudoStorage = new GameObject("PseudoStorage");
            pseudoStorage.transform.SetParent(transform);

            height = 6;
            width = 8;
            storageLabel = "MP3 PLAYER";
            storageRoot = pseudoStorage.AddComponent<ChildObjectIdentifier>();

            base.Awake();

            container.containerType = (ItemsContainerType)581;
            container.Clear();

            Pickupable _1_Previous = CraftData.InstantiateFromPrefab(ControlItems._1_Previous).GetComponentInChildren<Pickupable>();
            Pickupable _2_1_Play = CraftData.InstantiateFromPrefab(ControlItems._2_1_Play).GetComponentInChildren<Pickupable>();
            Pickupable _3_Next = CraftData.InstantiateFromPrefab(ControlItems._3_Next).GetComponentInChildren<Pickupable>();
            Pickupable _4_1_EnableRepeat = CraftData.InstantiateFromPrefab(ControlItems._4_1_EnableRepeat).GetComponentInChildren<Pickupable>();
            Pickupable _5_1_EnableShuffle = CraftData.InstantiateFromPrefab(ControlItems._5_1_EnableShuffle).GetComponentInChildren<Pickupable>();
            Pickupable _6_ExitInterface = CraftData.InstantiateFromPrefab(ControlItems._6_ExitInterface).GetComponentInChildren<Pickupable>();

            ControlItems.CorrespondingAudioPlayers.Add(_1_Previous, player);
            ControlItems.CorrespondingAudioPlayers.Add(_2_1_Play, player);
            ControlItems.CorrespondingAudioPlayers.Add(_3_Next, player);
            ControlItems.CorrespondingAudioPlayers.Add(_4_1_EnableRepeat, player);
            ControlItems.CorrespondingAudioPlayers.Add(_5_1_EnableShuffle, player);
            ControlItems.CorrespondingAudioPlayers.Add(_6_ExitInterface, player);

            container.AddItem(_1_Previous);
            container.AddItem(_2_1_Play);
            container.AddItem(_3_Next);
            container.AddItem(_4_1_EnableRepeat);
            container.AddItem(_5_1_EnableShuffle);
            container.AddItem(_6_ExitInterface);

            container.isAllowedToAdd = new IsAllowedToAdd((p, b) => false);
            container.isAllowedToRemove = new IsAllowedToRemove((p, b) => false);
        }
    }

    public static class ControlItems
    {
        public static TechType _1_Previous;
        public static TechType _2_1_Play;
        public static TechType _2_2_Pause;
        public static TechType _3_Next;
        public static TechType _4_1_EnableRepeat;
        public static TechType _4_2_DisableRepeat;
        public static TechType _5_1_EnableShuffle;
        public static TechType _5_2_DisableShuffle;
        public static TechType _6_ExitInterface;

        public static Dictionary<Pickupable, OSTAudioPlayer> CorrespondingAudioPlayers = new Dictionary<Pickupable, OSTAudioPlayer>();

        public static void Patch()
        {
            AddTechTypes();
            SetTechTypeProperties();
            AddActions();
        }

        public static void AddTechTypes()
        {
            _1_Previous = TechTypeHandler.AddTechType("MP3Player_Controls_Previous", "Previous", "Previous song");
            _2_1_Play = TechTypeHandler.AddTechType("MP3Player_Controls_Play", "Play/Pause", "Current status: <color=#C21807FF>paused</color>");
            _2_2_Pause = TechTypeHandler.AddTechType("MP3Player_Controls_Pause", "Pause/Pause", "Current status: <color=#4CBB17FF>playing</color>");
            _3_Next = TechTypeHandler.AddTechType("MP3Player_Controls_Next", "Next", "Next song");
            _4_1_EnableRepeat = TechTypeHandler.AddTechType("MP3Player_Controls_EnableRepeat", "Repeat", "Current status: <color=#C21807FF>disabled</color>");
            _4_2_DisableRepeat = TechTypeHandler.AddTechType("MP3Player_Controls_DisableRepeat", "Repeat", "Current status: <color=#4CBB17FF>enabled</color>");
            _5_1_EnableShuffle = TechTypeHandler.AddTechType("MP3Player_Controls_EnableShuffle", "Shuffle", "Current status: <color=#C21807FF>disabled</color>");
            _5_2_DisableShuffle = TechTypeHandler.AddTechType("MP3Player_Controls_DisableShuffle", "Shuffle", "Current status: <color=#4CBB17FF>enabled</color>");
            _6_ExitInterface = TechTypeHandler.AddTechType("MP3Player_Controls_ExitInterface", "Exit", "Close the interface");
        }
        public static void SetTechTypeProperties()
        {
            CraftDataHandler.SetItemSize(_1_Previous, 2, 2);
            CraftDataHandler.SetItemSize(_2_1_Play, 2, 2);
            CraftDataHandler.SetItemSize(_2_2_Pause, 2, 2);
            CraftDataHandler.SetItemSize(_3_Next, 2, 2);
            CraftDataHandler.SetItemSize(_4_1_EnableRepeat, 2, 2);
            CraftDataHandler.SetItemSize(_4_2_DisableRepeat, 2, 2);
            CraftDataHandler.SetItemSize(_5_1_EnableShuffle, 2, 2);
            CraftDataHandler.SetItemSize(_5_2_DisableShuffle, 2, 2);
            CraftDataHandler.SetItemSize(_6_ExitInterface, 2, 2);
        }
        public static void AddActions()
        {
            ItemActionHelper.RegisterAction(MouseButton.Left, _1_Previous, _1_Previous_OnClick, "Use", true.ToPredicate<InventoryItem>());
            ItemActionHelper.RegisterAction(MouseButton.Left, _2_1_Play, _2_1_Play_OnClick, "Use", true.ToPredicate<InventoryItem>());
            ItemActionHelper.RegisterAction(MouseButton.Left, _2_2_Pause, _2_2_Pause_OnClick, "Use", true.ToPredicate<InventoryItem>());
            ItemActionHelper.RegisterAction(MouseButton.Left, _3_Next, _3_Next_OnClick, "Use", true.ToPredicate<InventoryItem>());
            ItemActionHelper.RegisterAction(MouseButton.Left, _4_1_EnableRepeat, _4_1_EnableRepeat_OnClick, "Use", true.ToPredicate<InventoryItem>());
            ItemActionHelper.RegisterAction(MouseButton.Left, _4_2_DisableRepeat, _4_2_DisableRepeat_OnClick, "Use", true.ToPredicate<InventoryItem>());
            ItemActionHelper.RegisterAction(MouseButton.Left, _5_1_EnableShuffle, _5_1_EnableShuffle_OnClick, "Use", true.ToPredicate<InventoryItem>());
            ItemActionHelper.RegisterAction(MouseButton.Left, _5_2_DisableShuffle, _5_2_DisableShuffle_OnClick, "Use", true.ToPredicate<InventoryItem>());
            ItemActionHelper.RegisterAction(MouseButton.Left, _6_ExitInterface, _6_ExitInterface_OnClick, "Use", true.ToPredicate<InventoryItem>());
        }

        #region Actions

        public static void _1_Previous_OnClick(InventoryItem item)
        {

        }
        public static void _2_1_Play_OnClick(InventoryItem item)
        {

        }
        public static void _2_2_Pause_OnClick(InventoryItem item)
        {

        }
        public static void _3_Next_OnClick(InventoryItem item)
        {

        }
        public static void _4_1_EnableRepeat_OnClick(InventoryItem item)
        {

        }
        public static void _4_2_DisableRepeat_OnClick(InventoryItem item)
        {

        }
        public static void _5_1_EnableShuffle_OnClick(InventoryItem item)
        {

        }
        public static void _5_2_DisableShuffle_OnClick(InventoryItem item)
        {

        }
        public static void _6_ExitInterface_OnClick(InventoryItem item)
        {
            Object.FindObjectsOfType<OSTAudioPlayer>().Do(p => p.IsOpen = false);
            //CorrespondingAudioPlayers[item.item].IsOpen = false;
            Player.main.GetPDA().Close();
            Player.main.GetPDA().Open(PDATab.Inventory);
        }

        #endregion
    }

    public static class Patches
    {
        [HarmonyPatch(typeof(ItemsContainer))]
        [HarmonyPatch("IItemsContainer.UpdateContainer")]
        public static class ItemsContainer_IItemsContainer_UpdateContainer
        {
            [HarmonyPrefix]
            public static bool Prefix(ItemsContainer __instance)
            {
                if (__instance.containerType == (ItemsContainerType)581) return false;
                return true;
            }
        }

        [HarmonyPatch(typeof(PDA))]
        [HarmonyPatch("Close")]
        public static class PDA_Close
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                Object.FindObjectsOfType<OSTAudioPlayer>().Do(p => p.IsOpen = false);
            }
        }
    }
}
