using SMLHelper.V2.Handlers;
using System.Collections.Generic;
using System.Reflection;

namespace AlexejheroYTB.Common
{
    public class LanguageHelper
    {
        public static Dictionary<Assembly, uint> counters = new Dictionary<Assembly, uint>();
        public static List<LanguageHelper> helpers = new List<LanguageHelper>();

        public string Label;
        public string Id;

        public LanguageHelper(string label) : this(label, GenerateID(Assembly.GetCallingAssembly())) { }
        public LanguageHelper(string label, string id)
        {
            Label = label;
            Id = id;
            helpers.Add(this);
            LanguageHandler.SetLanguageLine(Id, Label);
        }

        public static string GenerateID(Assembly assembly)
        {
            if (!counters.ContainsKey(assembly)) counters.Add(assembly, 0);
            counters[assembly]++;
            return $"{assembly.GetName().Name}-auto-generated-label-{counters[assembly]}";
        }

        public static implicit operator LanguageHelper(string s)
            => new LanguageHelper(s);
        public static implicit operator string(LanguageHelper th)
            => th.Id;
    }
}
