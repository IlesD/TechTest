using System.Collections.Generic;

namespace Product.Api.Models
{
    public class ClientProductsResponse
    {
        public List<Product> Products { get; set; }

        public ApiKeys ApiKeys { get; set; }
    }
}