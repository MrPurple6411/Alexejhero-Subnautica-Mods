using AlexejheroYTB.Common;
using SMLHelper.V2.Handlers;
using UnityEngine;

namespace AlexejheroYTB.SonyWalkman
{
    public static class Mod
    {
        public static readonly PathHelper DLL_PATH = PathHelper.GetDLLPath();
        public static readonly PathHelper ASSETS_PATH = DLL_PATH + "Assets";
        public static readonly Texture2D USER_AVATAR = SteamUtils.GetAvatar();

        public static void Patch()
        {
            HarmonyHelper.Patch();
            Item.Load();
        }
    }

    public static class Language
    {
        public const string ID = "sonywalkman";
        public const string DISPLAY_NAME = "Sony Walkman";
        public const string TOOLTIP = "A music player your grandpa gave to you when you where a child.\nCan play music from the \"OST\" folder.\nI recommend Abandon Ship.";
        public const string LEFT_CLICK_TOOLTIP = "play / pause";
        public const string MIDDLE_CLICK_TOOLTIP = "change song";
    }

    public static class Item
    {
        public static readonly TechType TECH_TYPE = TechTypeHandler.AddTechType(Language.ID, Language.DISPLAY_NAME, Language.TOOLTIP, SPRITE);
        public static readonly TechDataHelper TECH_DATA = TECH_TYPE;
        public static readonly CraftTreeHelper CRAFTING_NODE = CraftTreeHelper.AddCraftingNode(TECH_TYPE, CraftTree.Type.Fabricator, "Personal/Equipment");
        //public static readonly Atlas.Sprite SPRITE = SMLHelper.V2.Utility.ImageUtils.LoadSpriteFromFile(Mod.ASSETS_PATH + "Radio.png");
        public static readonly Sprite SPRITE = Sprite.Create(Mod.USER_AVATAR, new Rect(Vector2.zero, Mod.USER_AVATAR.GetSize().ToVector2()), Vector2.zero);
        public static readonly ItemActionHelper LEFT_CLICK_ACTION = ItemActionHelper.RegisterAction(MouseButton.Left, TECH_TYPE, null, Language.LEFT_CLICK_TOOLTIP, true.ToPredicate<InventoryItem>());
        public static readonly ItemActionHelper MIDDLE_CLICK_ACTION = ItemActionHelper.RegisterAction(MouseButton.Middle, TECH_TYPE, null, Language.MIDDLE_CLICK_TOOLTIP, true.ToPredicate<InventoryItem>());

        public static void Load() { }
    }
}
