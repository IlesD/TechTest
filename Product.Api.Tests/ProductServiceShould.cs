using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Product.Api.Clients;
using Product.Api.Models;
using Product.Api.Services;

namespace Product.Api.Tests
{
    [TestClass]
    public class ProductServiceShould
    {
        private Mock<IProductClient> _productClientMock;
        // private Fixture _fixture;
        private string _okProductResponseString;

        [TestInitialize]
        public async Task Init()
        {
            _productClientMock = new Mock<IProductClient>();
            _okProductResponseString = await File.ReadAllTextAsync("../../../TestData/OkProductResponse.Json");
        }

        [TestMethod]
        public async Task Return_Products_When_ProductClient_Returns_OkResult()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StringContent(_okProductResponseString);
            
            _productClientMock
                .Setup(m => m.GetProducts(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);

            var productService = new ProductService(_productClientMock.Object);

            // Act
            var productsResponse = await productService.GetProducts(null, null, null);

            // Assert
            productsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            productsResponse.Response.Products.Any().Should().BeTrue();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.InternalServerError, DisplayName = "500 response")]
        [DataRow(HttpStatusCode.BadRequest, DisplayName = "400 response")]
        [DataRow(HttpStatusCode.NotFound, DisplayName = "404 response")]
        [DataRow(HttpStatusCode.NoContent, DisplayName = "204 response")]
        public async Task Return_Error_When_ProductClient_DoesNotReturn_OkResult(HttpStatusCode statusCode)
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage(statusCode);
            
            _productClientMock
                .Setup(m => m.GetProducts(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);

            var productService = new ProductService(_productClientMock.Object);

            // Act
            var productsResponse = await productService.GetProducts(null, null, null);

            // Assert
            productsResponse.StatusCode.Should().Be(HttpStatusCode.FailedDependency);
            productsResponse.IsSuccessStatusCode.Should().BeFalse();
            productsResponse.Message.Should().Be("Sorry! Something's gone wrong");
            productsResponse.ErrorMessage.Should().Be("System.Net.Http.EmptyContent");
        }

        [TestMethod]
        public void AddDescriptionHighlights_Should_Highlight_Select_Words()
        {
            // Arrange
            var highlightFilters = "shirt,green,red";
            var products = JsonConvert.DeserializeObject<ClientProductsResponse>(_okProductResponseString)!.Products;
            
            var productService = new ProductService(_productClientMock.Object);

            // Act
            productService.AddDescriptionHighlights(products, highlightFilters);

            // Assert
            products[0].Description.Contains("<em>green</em>").Should().BeTrue();
            products[0].Description.Contains("<em>shirt</em>").Should().BeTrue();
            products[2].Description.Contains("<em>red</em>").Should().BeTrue();
            products[2].Description.Contains("<em>shirt</em>").Should().BeTrue();
            products[10].Description.Contains("<em>").Should().BeFalse();
            products[10].Description.Contains("</em>").Should().BeFalse();
        }

        [TestMethod]
        public void ParseProductFilters_Should_Return_ProductFilters()
        {
            // Arrange
            var products = JsonConvert.DeserializeObject<ClientProductsResponse>(_okProductResponseString)!.Products;
            var expectedSizes = new string[] { "small", "medium", "large" };
            var expectedHighlights = new string[] { "shirt", "hat", "trouser", "green", "blue", "red", "belt", "bag", "shoe", "tie" };

            var productService = new ProductService(_productClientMock.Object);

            // Act
            var filters = productService.ParseProductFilters(products);

            // Assert
            filters.MinPrice.Should().Be(10);
            filters.MaxPrice.Should().Be(25);
            filters.Sizes.Should().Contain(expectedSizes);
            filters.Highlights.Should().Contain(expectedHighlights);
        }

        [TestMethod]
        public void GetDescriptionsHighlights_Should_Return_Highlights()
        {
            // Arrange
            var products = JsonConvert.DeserializeObject<ClientProductsResponse>(_okProductResponseString)!.Products;
            var expectedHighlights = new string[] { "shirt", "hat", "trouser", "green", "blue", "red", "belt", "bag", "shoe", "tie" };

            var productService = new ProductService(_productClientMock.Object);

            // Act
            var highlights = productService.GetDescriptionsHighlights(products);

            // Assert
            highlights.Should().Contain(expectedHighlights);
        }

        [TestMethod]
        public void FilterProducts_Should_Return_ProductFilters()
        {
            // Arrange
            var productsResponse = JsonConvert.DeserializeObject<ClientProductsResponse>(_okProductResponseString)!;
            var expectedHighlights = new string[] { "shirt", "hat", "trouser", "green", "blue", "red", "belt", "bag", "shoe", "tie" };

            var productService = new ProductService(_productClientMock.Object);

            // Act
            productService.FilterProducts(productsResponse, 20, "small");

            // Assert
            productsResponse.Products.All(p => p.Sizes.Contains("small")).Should().BeTrue();
        }
        
    }
}