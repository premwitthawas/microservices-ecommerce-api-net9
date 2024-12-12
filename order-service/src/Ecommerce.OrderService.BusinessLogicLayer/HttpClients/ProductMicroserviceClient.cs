using System.Net;
using System.Net.Http.Json;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;

namespace Ecommerce.OrderService.BusinessLogicLayer.HttpClients
{
    public class ProductMicroserviceClient
    {
        private readonly HttpClient _httpClient;

        public ProductMicroserviceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/products/search/{productId}");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException("Bad request", null, HttpStatusCode.BadRequest);
                }
                else
                {
                    throw new HttpRequestException("Internal server error", null, HttpStatusCode.InternalServerError);
                }
            };
            ProductDto? product = await response.Content.ReadFromJsonAsync<ProductDto>();
            if (product == null)
            {
                throw new ArgumentException("Invalid Product ID");
            }
            return product;
        }
    }
}