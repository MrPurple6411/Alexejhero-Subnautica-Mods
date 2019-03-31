using AlexejheroYTB.Common;
using Harmony;
using System;
using System.Reflection;

namespace AlexejheroYTB.CyclopsInceptionUpgrade
{
    public static class QMod
    {
        public static string assembly = Assembly.GetCallingAssembly().GetName().Name;

        public static void Patch()
        {
            try
            {
                HarmonyHelper.Patch();

                Logger.Log("Patched successfully!");
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Initializing);
            }
        }
    }

    public static class Patches
    {
        
    }
}