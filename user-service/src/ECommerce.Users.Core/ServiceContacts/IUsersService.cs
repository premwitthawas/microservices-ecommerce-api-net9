using ECommerce.Users.Core.DTOs;

namespace ECommerce.Users.Core.ServiceContacts;


/// <summary>
/// Contact to be implemented by Users Service
/// </summary>
public interface IUsersService {
    /// <summary>
    /// Method to Login Service Layers
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<AuthenticationResponse?> Login(LoginRequest loginRequest);
    /// <summary>
    /// Method to Register Service Layers
    /// </summary>
    /// <param name="registerRequest"></param>
    /// <returns></returns>
    Task<AuthenticationResponse?> RegisterAsync(RegisterRequest registerRequest);
};