using Harmony;
using ModdingAdventCalendar.Utility;
using System;
using System.Reflection;

namespace ModdingAdventCalendar.Turrets
{
    public static class QMod
    {
        public static void Patch()
        {
            try
            {
                HarmonyInstance.Create("moddingadventcalendar.turrets").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Logger.Exception(e, LoggedWhen.Patching);
            }
        }
    }
}