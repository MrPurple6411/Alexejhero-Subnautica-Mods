using SMLHelper.V2.Handlers;

namespace AlexejheroYTB.Common
{
    public static class SpriteHelper
    {
        public static string RegisterSprite(TechType techtype, string path)
        {
            SpriteHandler.RegisterSprite(techtype, path);
            return path;
        }
    }
}
