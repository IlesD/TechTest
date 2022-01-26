using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Product.Api.Models;

namespace Product.Api.Services
{
    public interface IProductService
    {
        public Task<ActionResponse<FilteredProductsResponse>> GetProducts(int? maxPriceFilter, string? sizeFilter, string? highlightFilter);
    }
}