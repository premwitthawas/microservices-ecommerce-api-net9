using System.Net;
using System.Net.Http.Json;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;

namespace Ecommerce.OrderService.BusinessLogicLayer.HttpClients;


public class UsersMicroserviceClient
{
    private readonly HttpClient _httpClient;
    public UsersMicroserviceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserDto?> GetUserByUserID(Guid? userID)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"/api/users/{userID}");
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new HttpRequestException("Bad Request", null, HttpStatusCode.BadRequest);
            }
            else
            {
                throw new HttpRequestException("http request failed with status code", null, response.StatusCode);
            }
        }
        UserDto? user = await response.Content.ReadFromJsonAsync<UserDto>();
        if (user == null)
        {
            throw new ArgumentException("Invalid response from user microservice");
        }
        return user;
    }
}