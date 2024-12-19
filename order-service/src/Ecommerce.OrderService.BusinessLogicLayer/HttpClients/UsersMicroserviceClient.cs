using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using Ecommerce.OrderService.BusinessLogicLayer.Policies;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace Ecommerce.OrderService.BusinessLogicLayer.HttpClients;


public class UsersMicroserviceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersMicroserviceClient> _logger;
    private readonly IUserMicroservicePolicies _userMicroservicePolicies;
    private readonly IDistributedCache _distributedCache;
    public UsersMicroserviceClient(HttpClient httpClient, ILogger<UsersMicroserviceClient> logger, IUserMicroservicePolicies userMicroservicePolicies, IDistributedCache distributedCache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _userMicroservicePolicies = userMicroservicePolicies;
        _distributedCache = distributedCache;
    }

    public async Task<UserDto?> GetUserByUserID(Guid? userID)
    {
        try
        {
            string cacheKey = $"user:{userID}";
            string? cacheUserById = await _distributedCache.GetStringAsync(cacheKey);
            if (cacheUserById != null)
            {
                _logger.LogInformation("User retrieved from cache");
                UserDto? userFromCache = JsonSerializer.Deserialize<UserDto>(cacheUserById);
                return userFromCache;
            }
            HttpResponseMessage response = await _userMicroservicePolicies.GetCombinedPolicy().ExecuteAsync((ct) => _httpClient.GetAsync($"/api/users/{userID}", ct), CancellationToken.None);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    UserDto? userFromFallback = await response.Content.ReadFromJsonAsync<UserDto>();
                    if (userFromFallback == null)
                    {
                        throw new NotImplementedException("Fallback policy failed was not implemented correctly");
                    }
                    return userFromFallback;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException("Bad Request", null, HttpStatusCode.BadRequest);
                }
                else
                {
                    // throw new HttpRequestException("Internal server error", null, response.StatusCode);
                    return this.GetDefaultUser();
                }
            }
            UserDto? user = await response.Content.ReadFromJsonAsync<UserDto>();
            if (user == null)
            {
                throw new ArgumentException("Invalid response from user microservice");
            }
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
               .SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddMinutes(5))
               .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
            await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(user), options);
            _logger.LogInformation("User retrieved from database");
            return user;
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "Timeout");
            return this.GetDefaultUser(); // คืนค่าผู้ใช้เริ่มต้น
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "Circuit breaker is open");
            return this.GetDefaultUser(); // คืนค่าผู้ใช้เริ่มต้น
        }
    }

    private UserDto GetDefaultUser()
    {
        return new UserDto(
            UserID: Guid.Empty,
            Email: "Temporarily Unavailable",
            PersonName: "Temporarily Unavailable",
            Gender: "Temporarily Unavailable"
        );
    }
}