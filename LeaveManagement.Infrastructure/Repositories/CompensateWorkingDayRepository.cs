using Dapper;
using LeaveManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Infrastructure.Repositories
{
    public class CompensateWorkingDayRepository : ICompensateWorkingDayRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public CompensateWorkingDayRepository(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<List<DateTime>> GetCompensateDaysInRange(DateTime from, DateTime to)
        {
            var sql = @"SELECT CompensateDate FROM CompensateWorkingDays WHERE CompensateDate BETWEEN @from AND @to";
            var result = await _connection.QueryAsync<DateTime>(sql, new { from, to }, transaction: _transaction);
            return result.ToList();
        }
    }
}
