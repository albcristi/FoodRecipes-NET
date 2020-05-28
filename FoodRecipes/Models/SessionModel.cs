using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodRecipes.Models
{
    public class SessionModel
    {
        public Int32 id { get; set; }

        public Int32 user_id { get; set; }

        public DateTime ttl { get; set; }

        public String token { get; set; }
    }
}
