using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Product.Api.Services;

namespace Product.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IProductService _productService;

        public SearchController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(
            [FromQuery(Name = "maxPrice")]int? maxPriceFilter, 
            [FromQuery(Name = "size")]string? sizeFilter, 
            [FromQuery(Name = "highlight")]string? highlightFilter)
        {
            var productsResponse = await _productService.GetProducts(maxPriceFilter, sizeFilter, highlightFilter);

            return productsResponse.IsSuccessStatusCode 
            ? Ok(productsResponse.Response) 
            : BadRequest(productsResponse);
        }
    }
}