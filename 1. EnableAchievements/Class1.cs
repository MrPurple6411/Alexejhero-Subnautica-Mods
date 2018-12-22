using Harmony;
using ModdingAdventCalendar.Utility;
using System.Reflection;

namespace ModdingAdventCalendar.Deconstructor
{
    public static class QMod
    {
        public static void Patch()
        {
            
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
                if (typeof(GameAchievements).GetMethod("GetPlatformId", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { id }) is string platformId)
                {
                    PlatformUtils.main.GetServices().UnlockAchievement(platformId);
                }
                return false;
            }
        }
    }
}