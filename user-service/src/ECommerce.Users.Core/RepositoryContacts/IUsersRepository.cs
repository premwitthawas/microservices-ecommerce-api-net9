using ECommerce.Users.Core.Entities;

namespace ECommerce.Users.Core.RepositoryContacts;

/// <summary>
/// Contact to be implemented by Users Repository
/// </summary>
public interface IUsersRepository
{
    /// <summary>
    /// Method Add User
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<AppplicaitonUser?> AddUser(AppplicaitonUser user);
    /// <summary>
    /// Method Get User By Email AND Password
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<AppplicaitonUser?> GetUserByEmailAndPassword(string? email, string? password);
};