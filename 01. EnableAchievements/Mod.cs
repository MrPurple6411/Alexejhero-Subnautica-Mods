using Harmony;
using ModdingAdventCalendar.Utility;
using System;
using System.Reflection;

namespace ModdingAdventCalendar.EnableAchievements
{
    public static class QMod
    {
        public static string assembly;

        public static void Patch()
        {
            try
            {
                assembly = Assembly.GetExecutingAssembly().GetName().FullName;

                HarmonyInstance.Create("moddingadventcalendar.enableachievements").PatchAll(Assembly.GetExecutingAssembly());

                Console.WriteLine($"[{assembly}] Patched successfully!");
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Patching);
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
                        Console.WriteLine($"[{QMod.assembly}] Force unlocked achievement {platformId}!");
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