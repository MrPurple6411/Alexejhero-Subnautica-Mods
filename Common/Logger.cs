using System;
using System.Reflection;

namespace AlexejheroYTB.Common
{
    public enum LoggedWhen
    {
        None = -1,
        Initializing,
        InPatch,
        Options,
    }

    public static class Logger
    {
        public static void Log(object toLog, string assembly = null)
        {
            assembly = assembly ?? Assembly.GetCallingAssembly().GetName().Name;
            Console.WriteLine($"[{assembly}] {toLog.ToString()}");
        }

        public static void Exception(Exception e, LoggedWhen loggedWhen = LoggedWhen.None, string assembly = null)
        {
            assembly = assembly ?? Assembly.GetCallingAssembly().GetName().Name;
            Console.WriteLine($"[{assembly}] {loggedWhen.Message()}");
            Console.WriteLine($"[{assembly}] Exception: {e.ToString()}");
        }

        public static string Message(this LoggedWhen loggedWhen)
        {
            switch (loggedWhen)
            {
                case LoggedWhen.Initializing:
                    return "An error occurred while creating harmony patch!";
                case LoggedWhen.InPatch:
                    return "An error occurred in a patched method!";
                case LoggedWhen.Options:
                    return "An error occurred while creating/changing mod options!";
                default:
                    return "An error has occurred!";
            }
        }
    }
}
