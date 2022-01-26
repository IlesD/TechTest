namespace Product.Api.Models
{
    public class ProductFilters
    {
        public int MinPrice { get; set; }

        public int MaxPrice { get; set; }

        public string[] Sizes { get; set; }

        public string[] Highlights { get; set; }
    }
}