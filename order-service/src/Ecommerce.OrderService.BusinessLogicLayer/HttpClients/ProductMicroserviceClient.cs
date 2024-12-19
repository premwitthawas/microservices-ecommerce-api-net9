using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.Bulkhead;

namespace Ecommerce.OrderService.BusinessLogicLayer.HttpClients
{
    public class ProductMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductMicroserviceClient> _logger;
        private readonly IDistributedCache _distributedCache;

        public ProductMicroserviceClient(HttpClient httpClient, ILogger<ProductMicroserviceClient> logger, IDistributedCache distributedCache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
        {
            try
            {
                string cacheKey = $"product:{productId}";
                string? cacheProductById = await _distributedCache.GetStringAsync(cacheKey);
                if (cacheProductById != null)
                {
                    _logger.LogInformation("Product retrieved from cache");
                    ProductDto? productFromCache = JsonSerializer.Deserialize<ProductDto>(cacheProductById);
                    return productFromCache;
                }
                HttpResponseMessage response = await _httpClient.GetAsync($"/api/products/search/{productId}");
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        ProductDto? productFromFallback = await response.Content.ReadFromJsonAsync<ProductDto>();
                        if (productFromFallback == null)
                        {
                            throw new NotImplementedException("Fallback policy failed was not implemented correctly");
                        }
                        return productFromFallback;
                    }
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
                 DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
                .SetSlidingExpiration(TimeSpan.FromSeconds(100));
                await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(product), options);
                _logger.LogInformation("Product retrieved from database");
                return product;
            }
            catch (BulkheadRejectedException ex)
            {
                _logger.LogError(ex, "Bulkhead policy triggered");
                return GetDefaultProduct();
            }
        }
        static ProductDto GetDefaultProduct()
        {
            return new ProductDto(ProductID: Guid.Empty,
                                  ProductName: "Temporarily Unavailable",
                                  Category: "Temporarily Unavailable",
                                  UnitPrice: 0,
                                  QuantityInStock: 0);
        }
    }
}