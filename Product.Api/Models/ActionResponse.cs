using System.Net;

namespace Product.Api.Models
{
    public class ActionResponse<T> : ActionResponse 
    {
        public T Response { get; set; }
    }

    public class ActionResponse
    {
        public string? ErrorMessage { get; set; }
        public bool IsSuccessStatusCode => (int)StatusCode >= 200 && (int)StatusCode <= 299;
        public string? Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}