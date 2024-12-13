using Polly;

namespace Ecommerce.OrderService.BusinessLogicLayer.Policies;


public interface IUserMicroservicePolicies {
    IAsyncPolicy<HttpResponseMessage> GetRetryPolicy();
    IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy();
}