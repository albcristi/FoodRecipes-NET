using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodRecipes.Models
{
    public class RecipeModel
    {
        public Int32 id { get; set; }

        public String chef_name { get; set; }

        public String name { get; set; }

        public String description { get; set; }

        public String type { get; set; }

        public String steps { get; set; }

    }
}
