using Harmony;
using System.Collections.Generic;
using System.Reflection;

namespace AlexejheroYTB.Common
{
    public static class HarmonyHelper
    {
        public static readonly List<Assembly> patchedAssemblies = new List<Assembly>();

        public static void Patch()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            if (patchedAssemblies.Contains(assembly)) return;

            HarmonyInstance.Create($"alexejheroytb.{assembly.GetName().Name.ToLower()}").PatchAll(assembly);
        }
    }
}
