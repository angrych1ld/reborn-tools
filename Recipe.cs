using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RebornTools
{
    public class Recipe
    {
        public int Yields { get; set; }
        public int SuccessRate { get; set; }
        public List<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
        public int CraftPriceMin { get; set; }
        public int CraftPriceMax { get; set; }
    }
}
