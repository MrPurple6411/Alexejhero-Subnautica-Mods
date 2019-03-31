using SMLHelper.V2.Crafting;
using System.Collections.Generic;
using UnityEngine;

namespace AlexejheroYTB.CyclopsInceptionUpgrade
{
    internal class InceptionModule : CyclopsModule
    {
        internal InceptionModule()
            : base("CyclopsInceptionModule",
                  "Cyclops Inception Module",
                  "With this module you can dock a cyclops into a cyclops into a cyclops into a cyclops into a cyclops into a cyclops into a cyclops",
                  CraftTree.Type.Workbench,
                  new[] { "CyclopsMenu" },
                  TechType.Workbench)
        { }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.CyclopsThermalReactorModule);
            var obj = GameObject.Instantiate(prefab);

            return obj;
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                {
                    new Ingredient(TechType.Titanium, 1)
                })
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID) => InceptionModuleID = techTypeID;
    }
}
