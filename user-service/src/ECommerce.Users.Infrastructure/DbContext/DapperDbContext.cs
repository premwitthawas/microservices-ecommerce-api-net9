using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ECommerce.Users.Infrastructure.DbContext
{
    public class DapperDbContext
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _connection;
        public DapperDbContext(IConfiguration configuration)
        {
            string? host = Environment.GetEnvironmentVariable("PG_HOST");
            string? port = Environment.GetEnvironmentVariable("PG_PORT");
            string? username = Environment.GetEnvironmentVariable("PG_USER");
            string? password = Environment.GetEnvironmentVariable("PG_PASSWORD");
            string? database = Environment.GetEnvironmentVariable("PG_DB");
            _configuration = configuration;
            string? connectionString = _configuration.GetConnectionString("PostgresSQLConnection");
            connectionString = connectionString!.Replace("{PG_HOST}", host)
            .Replace("{PG_PORT}", port)
            .Replace("{PG_USER}", username)
            .Replace("{PG_PASSWORD}", password)
            .Replace("{PG_DB}", database);
            _connection = new NpgsqlConnection(connectionString);
        }
        public IDbConnection DbConnection => _connection;
    }
}