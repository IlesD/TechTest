using System.Collections.Generic;

namespace Product.Api.Models
{
    public class FilteredProductsResponse
    {
        public List<Product> Products { get; set; }

        public ProductFilters Filters { get; set; }
    }
}