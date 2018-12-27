using SMLHelper.V2.Assets;
using System;
using UnityEngine;

namespace ModdingAdventCalendar.Utility
{
    public class PrefabHelper : ModPrefab
    {
        public Func<GameObject> Function;

        public PrefabHelper(string classid, string prefabfilename, TechType techtype, Func<GameObject> function) : base(classid, prefabfilename, techtype)
        {
            Function = function;
        }

        public override GameObject GetGameObject()
        {
            return Function.Invoke();
        }
    }
}
