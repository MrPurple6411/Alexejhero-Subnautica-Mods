using SMLHelper.V2.Handlers;

namespace AlexejheroYTB.Common
{
    public class CraftTreeHelper
    {
        public string path;

        public CraftTreeHelper(string p) => path = p;

        public static CraftTreeHelper AddCraftingNode(TechType item, CraftTree.Type craftTree, string path)
        {
            CraftTreeHandler.AddCraftingNode(craftTree, item, path.Split('/', '\\'));
            return path;
        }

        public static implicit operator CraftTreeHelper(string s) => new CraftTreeHelper(s);
        public static implicit operator string(CraftTreeHelper cth) => cth.path;
    }
}
