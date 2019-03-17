using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using System.Linq;

namespace AlexejheroYTB.Common
{
    public class TechDataHelper : TechData
    {
        public TechDataHelper(TechType result, int amount, params TechType[] ingredients) : base()
        {
            Ingredients = ingredients.Select(techType => new Ingredient(techType, 1)).ToList();
            craftAmount = amount;
            CraftDataHandler.SetTechData(result, this);
        }

        public static implicit operator TechDataHelper(TechType techType)
            => new TechDataHelper(techType, 1);
    }
}
