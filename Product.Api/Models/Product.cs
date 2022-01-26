using System.Collections.Generic;

namespace Product.Api.Models
{
    public class Product
    {
        public string Title { get; set; }
    
        public int Price { get; set; }

        public string Description { get; set; }

        public List<string> Sizes { get; set; }
    }
}