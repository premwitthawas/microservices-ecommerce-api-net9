using Polly;

namespace Ecommerce.OrderService.BusinessLogicLayer.Policies;

public interface IProductMicroservicePolicies
{
    IAsyncPolicy<HttpResponseMessage> GetFallBackPolicy();
    IAsyncPolicy<HttpResponseMessage> GetBulkHeadIsolationPolicy();
}