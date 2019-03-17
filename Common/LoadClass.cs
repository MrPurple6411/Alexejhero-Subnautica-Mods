using Harmony;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AlexejheroYTB.Common
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class LoadClass : Attribute
    {
        public static void Load()
        {
            Assembly.GetCallingAssembly().GetTypes().Where((type) => IsDefined(type, typeof(LoadClass))).Do((type) => RuntimeHelpers.RunClassConstructor(type.TypeHandle));
        }
    }
}
