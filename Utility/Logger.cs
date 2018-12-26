﻿using System;
using System.Reflection;

namespace ModdingAdventCalendar.Utility
{
    public enum LoggedWhen
    {
        None = -1,
        Patching,
        InPatch,
        Options,
    }
    public class Logger
    {
        public static void Exception(Exception e, LoggedWhen loggedWhen = LoggedWhen.None)
        {
            string assembly = Assembly.GetCallingAssembly().GetName().FullName;
            switch (loggedWhen)
            {
                case LoggedWhen.Patching:
                    Console.WriteLine($"[{assembly}] An error occured while creating harmony patch!");
                    break;
                case LoggedWhen.InPatch:
                    Console.WriteLine($"[{assembly}] An error occured in a patched method!");
                    break;
                case LoggedWhen.Options:
                    Console.WriteLine($"[{assembly}] An error occured while creating/changing mod options!");
                    break;
                case LoggedWhen.None:
                default:
                    break;
            }
            Console.WriteLine($"[{assembly}] Exception thrown: {e.Message}");
            Console.WriteLine($"[{assembly}] Stacktrace: {e.StackTrace}");
        }
    }
}
