using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Infrastructure.Configuration
{
    public class DapperDbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
