using Azure.Core;
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
    //public class UserLeaveBalanceRepository : IUserLeaveBalanceRepository
    //{
    //    private readonly IDbConnectionFactory? _connectionFactory;
    //    private readonly IDbConnection? _connection;
    //    private readonly IDbTransaction? _transaction;

    //    // Mode 1: Query (inject vào DI cho query handler, không dùng transaction)
    //    public UserLeaveBalanceRepository(IDbConnectionFactory connectionFactory)
    //    {
    //        _connectionFactory = connectionFactory;
    //    }

    //    // Mode 2: Ghi/Transaction (inject qua UnitOfWork)
    //    public UserLeaveBalanceRepository(IDbConnection connection, IDbTransaction? transaction = null)
    //    {
    //        _connection = connection;
    //        _transaction = transaction;
    //    }

    //    public async Task<int> AddUserLeaveBalance(UserLeaveBalances dto)
    //    {
    //        var parameters = new DynamicParameters();
    //        parameters.Add("@UserId", dto.UserId);
    //        parameters.Add("@Year", dto.Year);
    //        parameters.Add("@LeaveDaysGranted", dto.LeaveDaysGranted);
    //        parameters.Add("@LeaveDaysTaken", dto.LeaveDaysTaken);
    //        parameters.Add("@LeaveDaysRemain", dto.LeaveDaysRemain);
    //        parameters.Add("@DaysToDeduct", dto.DaysToDeduct);
    //        parameters.Add("@DaysToReturn", dto.DaysToReturn);
    //        parameters.Add("@Action", "AddUserLeaveBalance");
    //        parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

    //        var sql = "sp_USERSLEAVEBALANCES";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateCommandConnection();
    //        await conn.ExecuteAsync(
    //            sql,
    //            parameters,
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return parameters.Get<int>("@Id");
    //    }

    //    public async Task<bool> DeductUserLeaveBalance(UserLeaveBalances dto)
    //    {
    //        var parameters = new DynamicParameters();
    //        parameters.Add("@UserId", dto.UserId);
    //        parameters.Add("@Year", dto.Year);
    //        parameters.Add("@LeaveDaysGranted", dto.LeaveDaysGranted);
    //        parameters.Add("@LeaveDaysTaken", dto.LeaveDaysTaken);
    //        parameters.Add("@LeaveDaysRemain", dto.LeaveDaysRemain);
    //        parameters.Add("@DaysToDeduct", dto.DaysToDeduct);
    //        parameters.Add("@DaysToReturn", dto.DaysToReturn);
    //        parameters.Add("@Action", "DeductUserLeaveBalance");
    //        parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

    //        var sql = "sp_USERSLEAVEBALANCES";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateCommandConnection();
    //        var affectedRows = await conn.ExecuteAsync(
    //            sql,
    //            parameters,
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return affectedRows > 0;
    //    }

    //    public async Task<UserLeaveBalances> GetUserLeaveBalance(int userId, int year)
    //    {
    //        var sql = "sp_USERSLEAVEBALANCES";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateQueryConnection();
    //        var result = await conn.QuerySingleOrDefaultAsync<UserLeaveBalances>(
    //            sql,
    //            new { UserId = userId, Year = year, Action = "GetUserLeaveBalance" },
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return result!;
    //    }

    //    public async Task<bool> ReturnUserLeaveBalance(UserLeaveBalances dto)
    //    {
    //        var parameters = new DynamicParameters();
    //        parameters.Add("@UserId", dto.UserId);
    //        parameters.Add("@Year", dto.Year);
    //        parameters.Add("@LeaveDaysGranted", dto.LeaveDaysGranted);
    //        parameters.Add("@LeaveDaysTaken", dto.LeaveDaysTaken);
    //        parameters.Add("@LeaveDaysRemain", dto.LeaveDaysRemain);
    //        parameters.Add("@DaysToDeduct", dto.DaysToDeduct);
    //        parameters.Add("@DaysToReturn", dto.DaysToReturn);
    //        parameters.Add("@Action", "ReturnUserLeaveBalance");
    //        parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

    //        var sql = "sp_USERSLEAVEBALANCES";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateCommandConnection();
    //        var affectedRows = await conn.ExecuteAsync(
    //            sql,
    //            parameters,
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return affectedRows > 0;
    //    }

    //    public async Task<bool> UpdateUserLeaveBalance(UserLeaveBalances dto)
    //    {
    //        var parameters = new DynamicParameters();
    //        parameters.Add("@UserId", dto.UserId);
    //        parameters.Add("@Year", dto.Year);
    //        parameters.Add("@LeaveDaysGranted", dto.LeaveDaysGranted);
    //        parameters.Add("@LeaveDaysTaken", dto.LeaveDaysTaken);
    //        parameters.Add("@LeaveDaysRemain", dto.LeaveDaysRemain);
    //        parameters.Add("@DaysToDeduct", dto.DaysToDeduct);
    //        parameters.Add("@DaysToReturn", dto.DaysToReturn);
    //        parameters.Add("@Action", "UpdateUserLeaveBalance");
    //        parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

    //        var sql = "sp_USERSLEAVEBALANCES";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateCommandConnection();
    //        var affectedRows = await conn.ExecuteAsync(
    //            sql,
    //            parameters,
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return affectedRows > 0;
    //    }
    //}


    public class UserLeaveBalanceRepository : IUserLeaveBalanceRepository
    {
        private readonly IDbConnectionFactory? _connectionFactory;
        private readonly IDbConnection? _connection;
        private readonly IDbTransaction? _transaction;

        // Mode 1: Query (inject vào DI cho query handler, không dùng transaction)
        public UserLeaveBalanceRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Mode 2: Ghi/Transaction (inject qua UnitOfWork)
        public UserLeaveBalanceRepository(IDbConnection connection, IDbTransaction? transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> AddUserLeaveBalance(UserLeaveBalances dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", dto.UserId);
            parameters.Add("@Year", dto.Year);
            parameters.Add("@LeaveDaysGranted", dto.LeaveDaysGranted);
            parameters.Add("@LeaveDaysTaken", dto.LeaveDaysTaken);
            parameters.Add("@LeaveDaysRemain", dto.LeaveDaysRemain);
            parameters.Add("@DaysToDeduct", dto.DaysToDeduct);
            parameters.Add("@DaysToReturn", dto.DaysToReturn);
            parameters.Add("@Action", "AddUserLeaveBalance");
            parameters.Add("@Id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            var sql = "sp_USERSLEAVEBALANCES";
            if (_connection != null)
            {
                await _connection.ExecuteAsync(
                    sql,
                    parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure
                    );

                return parameters.Get<int>("@Id");
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                await conn.ExecuteAsync(
                    sql,
                    parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure
                    );

                return parameters.Get<int>("@Id");
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public async Task<bool> DeductUserLeaveBalance(UserLeaveBalances dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", dto.UserId);
            parameters.Add("@Year", dto.Year);
            parameters.Add("@LeaveDaysGranted", dto.LeaveDaysGranted);
            parameters.Add("@LeaveDaysTaken", dto.LeaveDaysTaken);
            parameters.Add("@LeaveDaysRemain", dto.LeaveDaysRemain);
            parameters.Add("@DaysToDeduct", dto.DaysToDeduct);
            parameters.Add("@DaysToReturn", dto.DaysToReturn);
            parameters.Add("@Action", "DeductUserLeaveBalance");
            parameters.Add("@Id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            var sql = "sp_USERSLEAVEBALANCES";
            if (_connection != null)
            {
                await _connection.ExecuteAsync(
                    sql,
                    parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure
                );
                int resultId = parameters.Get<int>("@Id");
                return resultId > 0;
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                await conn.ExecuteAsync(
                    sql,
                    parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure
                );
                int resultId = parameters.Get<int>("@Id");
                return resultId > 0;
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public async Task<UserLeaveBalances> GetUserLeaveBalance(int userId, int year)
        {
            var sql = "sp_USERSLEAVEBALANCES";
            if (_connection != null)
            {
                var result = await _connection.QuerySingleOrDefaultAsync<UserLeaveBalances>(
                    sql,
                    new { UserId = userId, Year = year, Action = "GetUserLeaveBalance" },
                    transaction: _transaction
                );
                return result!;
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var result = await conn.QuerySingleOrDefaultAsync<UserLeaveBalances>(
                    sql,
                    new { UserId = userId, Year = year, Action = "GetUserLeaveBalance" },
                    transaction: _transaction
                );
                return result!;
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public async Task<bool> ReturnUserLeaveBalance(UserLeaveBalances dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", dto.UserId);
            parameters.Add("@Year", dto.Year);
            parameters.Add("@LeaveDaysGranted", dto.LeaveDaysGranted);
            parameters.Add("@LeaveDaysTaken", dto.LeaveDaysTaken);
            parameters.Add("@LeaveDaysRemain", dto.LeaveDaysRemain);
            parameters.Add("@DaysToDeduct", dto.DaysToDeduct);
            parameters.Add("@DaysToReturn", dto.DaysToReturn);
            parameters.Add("@Action", "ReturnUserLeaveBalance");
            parameters.Add("@Id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            var sql = "sp_USERSLEAVEBALANCES";
            if (_connection != null)
            {
                await _connection.ExecuteAsync(
                    sql,
                    parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure
                );
                int resultId = parameters.Get<int>("@Id");
                return resultId > 0;
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                await conn.ExecuteAsync(
                    sql,
                    parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure
                );
                int resultId = parameters.Get<int>("@Id");
                return resultId > 0;
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public async Task<bool> UpdateUserLeaveBalance(UserLeaveBalances dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", dto.UserId);
            parameters.Add("@Year", dto.Year);
            parameters.Add("@LeaveDaysGranted", dto.LeaveDaysGranted);
            parameters.Add("@LeaveDaysTaken", dto.LeaveDaysTaken);
            parameters.Add("@LeaveDaysRemain", dto.LeaveDaysRemain);
            parameters.Add("@DaysToDeduct", dto.DaysToDeduct);
            parameters.Add("@DaysToReturn", dto.DaysToReturn);
            parameters.Add("@Action", "UpdateUserLeaveBalance");
            parameters.Add("@Id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            var sql = "sp_USERSLEAVEBALANCES";
            if (_connection != null)
            {
                await _connection.ExecuteAsync(
                    sql,
                    parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure
                );
                int resultId = parameters.Get<int>("@Id");
                return resultId > 0;
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                await conn.ExecuteAsync(
                    sql,
                    parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure
                );
                int resultId = parameters.Get<int>("@Id");
                return resultId > 0;
            }
            throw new InvalidOperationException("Repository not initialized.");
        }
    }
}
