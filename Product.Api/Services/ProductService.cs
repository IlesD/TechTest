using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Product.Api.Clients;
using Product.Api.Models;

[assembly: InternalsVisibleTo("Product.Api.Tests")]

namespace Product.Api.Services
{
    public class ProductService : IProductService
    {
        private const string BaseUrl = "to_be_updated"; // TODO pull from app settings
        private const string GenericErrorMessage = "Sorry! Something's gone wrong";
        private readonly IProductClient _productClient;

        public ProductService(IProductClient productClient)
        {
            _productClient = productClient;
        }

        public async Task<ActionResponse<FilteredProductsResponse>> GetProducts(int? maxPriceFilter, string? sizeFilter,
            string? highlightFilter)
        {
            var productResult = await _productClient.GetProducts(BaseUrl);

            if (!productResult.IsSuccessStatusCode)
            {
                // var errorMessage = productResult.Content
                return new ActionResponse<FilteredProductsResponse>
                {
                    ErrorMessage = productResult.Content?.ToString() ?? 
                        $"GetProducts failure. Response content could not be read. Status code: {productResult.StatusCode}",
                    Message = GenericErrorMessage,
                    StatusCode = HttpStatusCode.FailedDependency
                };
            }

            var productContent = await productResult.Content.ReadFromJsonAsync<ClientProductsResponse>();

            if (productContent == null ||
                !productContent.Products.Any()) // If no results returned no need to filter them so return now
            {
                return new ActionResponse<FilteredProductsResponse>
                {
                    Response = new FilteredProductsResponse
                    {
                        Products = productContent?.Products ?? new List<Models.Product>()
                    },
                    StatusCode = HttpStatusCode.OK
                };
            }

            var productFilters = ParseProductFilters(productContent.Products);

            FilterProducts(productContent, maxPriceFilter, sizeFilter);
            
            AddDescriptionHighlights(productContent.Products, highlightFilter);

            return new ActionResponse<FilteredProductsResponse>
            {
                Response = new FilteredProductsResponse
                {
                    Products = productContent?.Products ?? new List<Models.Product>(),
                    Filters = productFilters
                },
                StatusCode = HttpStatusCode.OK
            };
        }

        internal void AddDescriptionHighlights(List<Models.Product> products, string? highlightFilter)
        {
            if (string.IsNullOrEmpty(highlightFilter)) return;
            
            var highlights = highlightFilter.Split(',');
            foreach (var highlight in highlights)
            {
                foreach (var product in products.Where(p => p.Description.Contains(highlight)))
                {
                    product.Description = product.Description.Replace(highlight, $"<em>{highlight}</em>"); 
                }
            }
        }

        // Create filter object based upon the data of the products returned by the client
        internal ProductFilters ParseProductFilters(List<Models.Product> productContent)
        {
            var filters = new ProductFilters
            {
                MinPrice = productContent.Min(p => p.Price),
                MaxPrice = productContent.Max(p => p.Price),
                Sizes = productContent.SelectMany(p => p.Sizes).Distinct().ToArray(),
                Highlights = GetDescriptionsHighlights(productContent)
            };

            return filters;
        }

        // Get each word from each description, find out how many times each word appears in all the descriptions,...
        // ..., select the ten most common words from the descriptions, excluding the most common five
        internal string[] GetDescriptionsHighlights(IEnumerable<Models.Product> productContent)
        {
            var words = productContent.SelectMany(p 
                => p.Description.ToLower().Replace(".", "").Split(' ')).ToList(); 
            // Assumed logic to remove only '.' as any other punctuation may be considered part of a word

            var distinctWords = words.Distinct().ToList();

            var wordCounts = distinctWords.ToDictionary(word 
                => word, word => words.Count(w => w == word));

            var highlights = wordCounts.OrderByDescending(pair => pair.Value).Skip(5).Take(10)
                .Select(p => p.Key).ToArray();

            return highlights;
        }

        // Filter products by price & size
        internal void FilterProducts(ClientProductsResponse productContent, int? maxPriceFilter, string? sizeFilter)
        {
            if (maxPriceFilter.HasValue) // If price filter provided, filter by price
            {
                productContent.Products = productContent.Products
                    .Where(p => p.Price <= maxPriceFilter.Value)
                    .ToList();
            }
            
            if (!string.IsNullOrEmpty(sizeFilter)) // If size filter provided, filter by size
            {
                productContent.Products = productContent.Products
                    .Where(p => p.Sizes.Contains(sizeFilter))
                    .ToList();
            }
        }
    }
}