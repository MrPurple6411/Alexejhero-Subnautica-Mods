using System;

namespace AlexejheroYTB.Common
{
    public static class ValueUtils
    {
        public static bool ToNormalBool(this bool? nullableBool)
        {
            if (nullableBool == null || nullableBool == false) return false;
            if (nullableBool == true) return true;
            return false;
        }

        public static Predicate<T> ToPredicate<T>(this bool @bool)
        {
            if (@bool == true) return (obj) => true;
            return (obj) => false;
        }
    }
}
