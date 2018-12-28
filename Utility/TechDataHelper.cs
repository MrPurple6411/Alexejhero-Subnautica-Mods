using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using System;
using System.Linq;

namespace ModdingAdventCalendar.Utility
{
    public class TechDataHelper : TechData
    {
        public TechDataHelper(TechType result, int count, params TechType[] ingredients) : base()
        {
            Ingredients = ingredients.Select(techType => new Ingredient(techType, 1)).ToList();
            craftAmount = count;
            CraftDataHandler.SetTechData(result, this);
        }

        public static implicit operator TechDataHelper(TechType techType)
            => new TechDataHelper(techType, 1);
    }
}
