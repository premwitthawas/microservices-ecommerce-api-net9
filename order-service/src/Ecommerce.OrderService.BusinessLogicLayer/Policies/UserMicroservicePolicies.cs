using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Ecommerce.OrderService.BusinessLogicLayer.Policies;

public class UserMicroservicePolicies : IUserMicroservicePolicies
{
    private readonly ILogger<UserMicroservicePolicies> _logger;
    private readonly IPollyPolicies _pollyPolicies;

    public UserMicroservicePolicies(ILogger<UserMicroservicePolicies> logger, IPollyPolicies pollyPolicies)
    {
        _logger = logger;
        _pollyPolicies = pollyPolicies;
    }
    public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
    {
        var retryPolicy = _pollyPolicies.GetRetryPolicy(3);
        var circuitBreakerPolicy = _pollyPolicies.GetCircuitBreakerPolicy(3, TimeSpan.FromSeconds(30));
        var timeOut = _pollyPolicies.GetTimeoutPolicy(TimeSpan.FromSeconds(2));
        return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeOut);
    }

}