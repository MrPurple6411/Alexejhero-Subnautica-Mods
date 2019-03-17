using AlexejheroYTB.Common;
using SMLHelper.V2.Handlers;

namespace AlexejheroYTB.SonyWalkman
{
    public static class Mod
    {
        public static readonly PathHelper DLL_PATH = PathHelper.GetDLLPath();
        public static readonly PathHelper ASSETS_PATH = DLL_PATH + "../Assets";

        public static void Patch()
        {
            HarmonyHelper.Patch();
            LoadClass.Load();
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

    [LoadClass]
    public static class Item
    {
        public static readonly TechType techType = TechTypeHandler.AddTechType(Language.id, Language.displayName, Language.tooltip);
        public static readonly TechDataHelper techData = techType;
        public static readonly CraftTreeHelper craftingNode = CraftTreeHelper.AddCraftingNode(techType, CraftTree.Type.Fabricator, "Personal/Equipment");
        public static readonly string sprite = SpriteHelper.RegisterSprite(techType, Mod.ASSETS_PATH + "radio.png");
        public static readonly ItemActionHelper leftClickAction = ItemActionHelper.RegisterAction(MouseButton.Left, techType, null, Language.leftClickTooltip, true.ToPredicate<InventoryItem>());
        public static readonly ItemActionHelper middleClickAction = ItemActionHelper.RegisterAction(MouseButton.Middle, techType, null, Language.middleClickTooltip, true.ToPredicate<InventoryItem>());

        public static void ON_LEFT_CLICK()
        {
            
        }
    }
}
