using LeaveManagement.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Infrastructure.Persistence.Database
{    

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateCommandConnection()
        {
            var connectionString = _configuration.GetConnectionString("CommandConnection");
            return new SqlConnection(connectionString);
        }

        public IDbConnection CreateQueryConnection()
        {
            var connectionString = _configuration.GetConnectionString("QueryConnection");
            return new SqlConnection(connectionString);
        }
    }
}
