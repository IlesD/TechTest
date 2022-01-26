using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Product.Api.Clients
{
    public interface IProductClient
    {
        public Task<HttpResponseMessage> GetProducts(string url);
    }
}