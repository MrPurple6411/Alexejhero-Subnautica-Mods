using Harmony;
using System.Reflection;

namespace AlexejheroYTB.Common
{
    public static class HarmonyHelper
    {
        public static void Patch()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            HarmonyInstance.Create($"alexejheroytb.{assembly.GetName().Name.ToLower()}").PatchAll(assembly);
        }
    }
}
