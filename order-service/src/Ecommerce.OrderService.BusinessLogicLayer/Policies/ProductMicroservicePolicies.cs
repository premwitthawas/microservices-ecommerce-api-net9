using System.Net;
using System.Text;
using System.Text.Json;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Bulkhead;

namespace Ecommerce.OrderService.BusinessLogicLayer.Policies;

public class ProductMicroservicePolicies : IProductMicroservicePolicies
{
    private readonly ILogger<ProductMicroservicePolicies> _logger;
    public ProductMicroservicePolicies(ILogger<ProductMicroservicePolicies> logger)
    {
        _logger = logger;
    }

    public IAsyncPolicy<HttpResponseMessage> GetBulkHeadIsolationPolicy()
    {
        return Policy.BulkheadAsync<HttpResponseMessage>(
            maxParallelization: 2, //concurrent requests
            maxQueuingActions: 40, //queued requests
            onBulkheadRejectedAsync: (ctx)=> {
                _logger.LogWarning("Bulkhead policy triggered");
                throw new BulkheadRejectedException("Bulkhead policy is full and can't accept more requests");
            }
        );
    }

    public IAsyncPolicy<HttpResponseMessage> GetFallBackPolicy()
    {
        return Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .FallbackAsync<HttpResponseMessage>(async (ctx) =>
        {
            _logger.LogWarning("Fallback policy triggered");
            ProductDto productDto = new ProductDto(ProductID: Guid.Empty,
            ProductName: "Temporarily Unavailable",
            Category: "Temporarily Unavailable",
            UnitPrice: 0,
            QuantityInStock: 0);
            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent(JsonSerializer.Serialize(productDto), Encoding.UTF8, "application/json")
            });
        });
    }
}