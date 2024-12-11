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
    public async Task<ApplicaitonUser?> AddUser(ApplicaitonUser user)
    {
        user.UserID = Guid.NewGuid();
        string query = @"
            INSERT INTO public.""Users"" (""UserID"", ""Email"", ""Password"", ""PersonName"" , ""Gender"")
            VALUES (@UserID, @Email, @Password, @PersonName, @Gender);";
        using var connection = _dbContext.DbConnection;
        int rowCountAffected = await connection.ExecuteAsync(query, user);
        if (rowCountAffected == 0)
        {
            return null;
        }
        return user;
    }

    public async Task<ApplicaitonUser?> GetUserByEmailAndPassword(string? email, string? password)
    {
        string query = @"SELECT *
                        FROM public.""Users""
                        WHERE ""Email"" = @Email AND ""Password"" = @Password;";
        var parameters = new { Email = email, Password = password };
        using var connection = _dbContext.DbConnection;
        var user = await connection.QueryFirstOrDefaultAsync<ApplicaitonUser>(query, parameters);
        return user;
    }

    public async Task<ApplicaitonUser?> GetUserByUserID(Guid? userId)
    {
        var query = @"SELECT *
                        FROM public.""Users""
                        WHERE ""UserID"" = @UserID;";
        var parameters = new { UserID = userId };
        using var connection = _dbContext.DbConnection;
        ApplicaitonUser? user = await connection.QueryFirstOrDefaultAsync<ApplicaitonUser>(query, parameters);
        if (user == null)
        {
            return null;
        }
        return user;
    }
}