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
            _configuration = configuration;

            string? connectionString = _configuration.GetConnectionString("PostgresSQLConnection");

            _connection = new NpgsqlConnection(connectionString);
        }
        public IDbConnection DbConnection => _connection;
    }
}