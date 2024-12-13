using Microsoft.Extensions.Logging;
using Polly;

namespace Ecommerce.OrderService.BusinessLogicLayer.Policies;

public class UserMicroservicePolicies : IUserMicroservicePolicies
{
    private readonly ILogger<UserMicroservicePolicies> _logger;
    public UserMicroservicePolicies(ILogger<UserMicroservicePolicies> logger)
    {
        _logger = logger;
    }

    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                     .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),
                        onRetry: (outcome, timespan, retryCount, context) =>
                        {
                            _logger.LogWarning($"Retry {retryCount} after {timespan.TotalSeconds} seconds.");
                        });
    }

    public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                     .CircuitBreakerAsync(1, TimeSpan.FromMinutes(2),
                        onBreak: (outcome, timespan) =>
                        {
                            _logger.LogError($"Circuit broken for {timespan.TotalMinutes} minutes.");
                        },
                        onReset: () =>
                        {
                            _logger.LogInformation("Circuit closed.");
                        });
    }
}