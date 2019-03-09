using AlexejheroYTB.Common;
using Harmony;
using System;
using System.Reflection;

namespace AlexejheroYTB.EnableAchievements
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
        [HarmonyPatch(typeof(GameAchievements), "Unlock")]
        public static class GameAchievements_Unlock
        {
            [HarmonyPrefix]
            public static bool Prefix(GameAchievements.Id id)
            {
                try
                {
                    if (typeof(GameAchievements).GetMethod("GetPlatformId", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { id }) is string platformId)
                    {
                        PlatformUtils.main.GetServices().UnlockAchievement(platformId);
                        Logger.Log($"[{QMod.assembly}] Force unlocked achievement {platformId}!", QMod.assembly);
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception(e, LoggedWhen.InPatch);
                }
                return false;
            }
        }
    }
}