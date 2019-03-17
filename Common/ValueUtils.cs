using System;
using UnityEngine;

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

        public static Sprite ToSprite(this Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }
}
