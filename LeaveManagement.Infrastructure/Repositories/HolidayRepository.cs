using Dapper;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Infrastructure.Repositories
{
    public class HolidayRepository : IHolidayRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public HolidayRepository(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<List<DateTime>> GetHolidaysInRange(DateTime from, DateTime to)
        {  
            var sql = @"SELECT HolidayDate FROM Holidays WHERE HolidayDate BETWEEN @from AND @to";
            var result = await _connection.QueryAsync<DateTime>(sql, new { from, to }, transaction: _transaction, commandType: CommandType.Text);
            return result.ToList();
        }
    }
}
