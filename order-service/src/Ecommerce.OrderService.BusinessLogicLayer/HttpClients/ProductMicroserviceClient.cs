using System.Net;
using System.Net.Http.Json;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using Microsoft.Extensions.Logging;
using Polly.Bulkhead;

namespace Ecommerce.OrderService.BusinessLogicLayer.HttpClients
{
    public class ProductMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductMicroserviceClient> _logger;

        public ProductMicroserviceClient(HttpClient httpClient, ILogger<ProductMicroserviceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
        {
            try
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
            catch (BulkheadRejectedException ex)
            {
                _logger.LogError(ex, "Bulkhead policy triggered");
                return new ProductDto(ProductID: Guid.Empty,
                                        ProductName: "Temporarily Unavailable",
                                        Category: "Temporarily Unavailable",
                                        UnitPrice: 0,
                                        QuantityInStock: 0);
            }
        }
    }
}