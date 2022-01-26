using System.Net.Http;
using System.Threading.Tasks;

namespace Product.Api.Clients
{
    public class MockyClient : IProductClient
    {
        public async Task<HttpResponseMessage> GetProducts(string url)
        {
            using var httpClient = new HttpClient();
            var result = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));

            return result;
        }
    }
}