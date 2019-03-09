using SMLHelper.V2.Assets;
using SMLHelper.V2.Handlers;
using System;
using UnityEngine;

namespace AlexejheroYTB.Common
{
    public class PrefabHelper
    {
        public Prefab prefab;

        public class Prefab : ModPrefab
        {
            public Func<GameObject> Function;

            public Prefab(string classid, string prefabfilename, TechType techtype) : base(classid, prefabfilename, techtype) { }

            public override GameObject GetGameObject()
            {
                return Function.Invoke();
            }
        }

        public PrefabHelper(string classid, string prefabfilename, TechType techtype, Func<GameObject> function)
        {
            Prefab prefab = new Prefab(classid, prefabfilename, techtype)
            {
                Function = function
            };
            this.prefab = prefab;
            PrefabHandler.RegisterPrefab(this);
        }

        public static implicit operator Prefab(PrefabHelper ph)
            => ph.prefab;
    }
}
