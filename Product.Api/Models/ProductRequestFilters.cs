using System.Collections.Generic;

namespace Product.Api.Models
{
    public class ProductRequestFilters
    {
        public int MaxPrice { get; set; }   // Could need to be made a decimal

        public string Size { get; set; }

        public List<string> Highlight { get; set; } 
    }
}