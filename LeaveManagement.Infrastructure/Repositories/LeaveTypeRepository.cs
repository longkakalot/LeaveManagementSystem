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
    //public class LeaveTypeRepository : ILeaveTypeRepository
    //{
    //    private readonly IDbConnectionFactory _connectionFactory;

    //    public LeaveTypeRepository(IDbConnectionFactory connectionFactory)
    //    {
    //        _connectionFactory = connectionFactory;
    //    }


    //    public Task<int> CreateAsync(LeaveType request)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async Task<List<LeaveType>> GetAllAsync()
    //    {
    //        using var connection = _connectionFactory.CreateCommandConnection();

    //        var query = "SELECT Id, Name FROM LeaveTypes";
    //        var result = await connection.QueryAsync<LeaveType>(query);
    //        return result.ToList();

    //    }
    //}

    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly IDbConnectionFactory? _connectionFactory;
        private readonly IDbConnection? _connection;
        private readonly IDbTransaction? _transaction;

        // Constructor CHO QUERY HANDLER (DI sẽ gọi constructor này)
        public LeaveTypeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Constructor CHO UNITOFWORK (ghi/transaction)
        public LeaveTypeRepository(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateAsync(LeaveType request)
        {
            if (_connection == null)
                throw new InvalidOperationException("Repository not initialized for write operations.");

            var sql = @"
            INSERT INTO LeaveTypes (Name)
            VALUES (@Name);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
        ";

            var id = await _connection.ExecuteScalarAsync<int>(
                sql,
                new { request.Name },
                transaction: _transaction // luôn truyền transaction
            );
            return id;
        }

        public async Task<List<LeaveType>> GetAllAsync()
        {
            // Dùng connection từ UnitOfWork (transaction), hoặc mở connection từ factory (chỉ đọc)
            if (_connection != null)
            {
                var result = await _connection.QueryAsync<LeaveType>(
                    "SELECT Id, Name FROM LeaveTypes",
                    transaction: _transaction
                );
                return result.ToList();
            }
            else if (_connectionFactory != null)
            {
                using var connection = _connectionFactory.CreateCommandConnection();
                var result = await connection.QueryAsync<LeaveType>(
                    "SELECT Id, Name FROM LeaveTypes"
                );
                return result.ToList();
            }
            throw new InvalidOperationException("Repository not initialized correctly");
        }
    }

}
