using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace Ecommerce.OrderService.BusinessLogicLayer.Policies;

public class PollyPolicies : IPollyPolicies
{
    private readonly ILogger<PollyPolicies> _logger;
    public PollyPolicies(ILogger<PollyPolicies> logger)
    {
        _logger = logger;
    }
    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
    {
        AsyncRetryPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
  .WaitAndRetryAsync(
     retryCount: retryCount,
     sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Delay between retries
     onRetry: (outcome, timespan, retryAttempt, context) =>
     {
         _logger.LogInformation($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds");
     });

        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak)
    {
        AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
.CircuitBreakerAsync(
   handledEventsAllowedBeforeBreaking: handledEventsAllowedBeforeBreaking, //Threshold for failed requests
   durationOfBreak: durationOfBreak, // Waiting time to be in "Open" state
   onBreak: (outcome, timespan) =>
   {
       _logger.LogInformation($"Circuit breaker opened for {timespan.TotalMinutes} minutes due to consecutive 3 failures. The subsequent requests will be blocked");
   },
   onReset: () =>
   {
       _logger.LogInformation($"Circuit breaker closed. The subsequent requests will be allowed.");
   });

        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout)
    {
        AsyncTimeoutPolicy<HttpResponseMessage> policy = Policy.TimeoutAsync<HttpResponseMessage>(timeout);

        return policy;
    }
};