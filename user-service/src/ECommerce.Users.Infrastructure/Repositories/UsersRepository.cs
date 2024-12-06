using Dapper;
using ECommerce.Users.Core.DTOs;
using ECommerce.Users.Core.Entities;
using ECommerce.Users.Core.RepositoryContacts;
using ECommerce.Users.Infrastructure.DbContext;

namespace ECommerce.Users.Infrastructure.Repositories;

internal class UsersRepository : IUsersRepository
{
    private readonly DapperDbContext _dbContext;
    public UsersRepository(DapperDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<AppplicaitonUser?> AddUser(AppplicaitonUser user)
    {
        user.UserID = Guid.NewGuid();
        string query = @"
            INSERT INTO public.""Users"" (""UserID"", ""Email"", ""Password"", ""PersonName"" , ""Gender"")
            VALUES (@UserID, @Email, @Password, @PersonName, @Gender);";
        int rowCountAffected = await _dbContext.DbConnection.ExecuteAsync(query, user);
        if (rowCountAffected == 0)
        {
            return null;
        }
        return user;
    }

    public async Task<AppplicaitonUser?> GetUserByEmailAndPassword(string? email, string? password)
    {
        string query = @"SELECT *
                        FROM public.""Users""
                        WHERE ""Email"" = @Email AND ""Password"" = @Password;";
        var parameters = new { Email = email, Password = password };
        var user = await _dbContext.DbConnection.QueryFirstOrDefaultAsync<AppplicaitonUser>(query, parameters);
        return user;
    }
}