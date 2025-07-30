using Dapper;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Infrastructure.Repositories
{
    //public class UserRepository : IUserRepository
    //{
    //    private readonly IDbConnectionFactory? _connectionFactory;
    //    private readonly IDbConnection? _connection;
    //    private readonly IDbTransaction? _transaction;

    //    Mode 1: Query(inject vào DI cho query handler, không dùng transaction)
    //    public UserRepository(IDbConnectionFactory connectionFactory)
    //    {
    //        _connectionFactory = connectionFactory;
    //    }

    //    Mode 2: Ghi/Transaction(inject qua UnitOfWork)
    //    public UserRepository(IDbConnection connection, IDbTransaction? transaction = null)
    //    {
    //        _connection = connection;
    //        _transaction = transaction;
    //    }

    //    public async Task<User> GetByIdAsync(int id)
    //    {
    //        var sql = "sp_USERS";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateQueryConnection();
    //        return await conn.QuerySingleOrDefaultAsync<User>(
    //            sql,
    //            new { UserId = id, Action = "GetByIdAsync" },
    //            commandType: CommandType.StoredProcedure
    //        ) ?? throw new Exception("User not found");
    //    }

    //    public async Task<IList<User>> FindApproverAsync(string maChucVu, string maPhongBan)
    //    {
    //        var maChucVuNew = $",{maChucVu},";
    //        var sql = "sp_USERS";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateQueryConnection();
    //        var result = await conn.QueryAsync<User>(
    //            sql,
    //            new { MaChucVu = maChucVuNew, MaPhongBan = maPhongBan, Action = "FindApproverAsyncByMaChucVuAndMaPhongBan" },
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return result.ToList();
    //    }

    //    public async Task<IList<User>> FindApproverAsyncByMaChucVu(string maChucVu)
    //    {
    //        var maChucVuNew = $",{maChucVu},";
    //        var sql = "sp_USERS";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateQueryConnection();
    //        var result = await conn.QueryAsync<User>(
    //            sql,
    //            new { MaChucVu = maChucVuNew, Action = "FindApproverAsyncByMaChucVu" },
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return result.ToList();
    //    }

    //    public async Task<User?> LoginAsync(string username, string hashedPassword)
    //    {
    //        var sql = "sp_USERS";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateQueryConnection();
    //        return await conn.QueryFirstOrDefaultAsync<User>(
    //            sql,
    //            new { Username = username, PasswordHash = hashedPassword, Action = "LoginByUserAndPass" },
    //            commandType: CommandType.StoredProcedure
    //        );
    //    }

    //    public async Task<User?> GetTotalLeaveDaysByUserIdAndYear(int userId, int year)
    //    {
    //        var sql = "sp_USERS";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateQueryConnection();
    //        return await conn.QueryFirstOrDefaultAsync<User>(
    //            sql,
    //            new { UserId = userId, YearInput = year, Action = "GetTotalLeaveDaysByUserIdAndYear" },
    //            commandType: CommandType.StoredProcedure
    //        );
    //    }
    //}


    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory? _connectionFactory;
        private readonly IDbConnection? _connection;
        private readonly IDbTransaction? _transaction;

        // Mode 1: Query (inject vào DI cho query handler, không dùng transaction)
        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Mode 2: Ghi/Transaction (inject qua UnitOfWork)
        public UserRepository(IDbConnection connection, IDbTransaction? transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        // Các hàm dưới đây sẽ tự động chọn connection phù hợp

        public async Task<User> GetByIdAsync(int id)
        {
            var sql = "sp_USERS";
            if (_connection != null)
            {
                return await _connection.QuerySingleOrDefaultAsync<User>(
                    sql,
                    new { UserId = id, Action = "GetByIdAsync" },
                    transaction: _transaction
                ) ?? throw new Exception("User not found");
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                return await conn.QuerySingleOrDefaultAsync<User>(
                    sql,
                    new { UserId = id, Action = "GetByIdAsync" }
                ) ?? throw new Exception("User not found");
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public async Task<IList<User>> FindApproverAsync(string maChucVu, string maPhongBan)
        {
            var maChucVuNew = $",{maChucVu},";
            var sql = "sp_USERS";

            if (_connection != null)
            {
                var result = await _connection.QueryAsync<User>(
                    sql,
                    new { MaChucVu = maChucVuNew, MaPhongBan = maPhongBan, Action = "FindApproverAsyncByMaChucVuAndMaPhongBan" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var result = await conn.QueryAsync<User>(
                    sql,
                    new { MaChucVu = maChucVuNew, MaPhongBan = maPhongBan, Action = "FindApproverAsyncByMaChucVuAndMaPhongBan" }
                );
                return result.ToList();
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public async Task<IList<User>> FindApproverAsyncByMaChucVu(string maChucVu)
        {
            var maChucVuNew = $",{maChucVu},";
            var sql = "sp_USERS";

            if (_connection != null)
            {
                var result = await _connection.QueryAsync<User>(
                    sql,
                    new { MaChucVu = maChucVuNew, Action = "FindApproverAsyncByMaChucVu" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var result = await conn.QueryAsync<User>(
                    sql,
                    new { MaChucVu = maChucVuNew, Action = "FindApproverAsyncByMaChucVu" }
                );
                return result.ToList();
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        /// <summary>
        /// Xác thực user theo username + hashed password (dùng cho Login)
        /// </summary>
        public async Task<User?> LoginAsync(string username, string hashedPassword)
        {
            var sql = "sp_USERS";

            if (_connection != null)
            {
                return await _connection.QueryFirstOrDefaultAsync<User>(
                    sql,
                    new { Username = username, PasswordHash = hashedPassword, Action = "LoginByUserAndPass" },
                    transaction: _transaction
                );
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                return await conn.QueryFirstOrDefaultAsync<User>(
                    sql,
                    new { Username = username, PasswordHash = hashedPassword, Action = "LoginByUserAndPass" }
                );
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public async Task<User?> GetTotalLeaveDaysByUserIdAndYear(int userId, int year)
        {
            var sql = "sp_USERS";

            if (_connection != null)
            {
                return await _connection.QueryFirstOrDefaultAsync<User>(
                    sql,
                    new { UserId = userId, YearInput = year, Action = "GetTotalLeaveDaysByUserIdAndYear" },
                    transaction: _transaction
                );
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                return await conn.QueryFirstOrDefaultAsync<User>(
                    sql,
                    new { UserId = userId, YearInput = year, Action = "GetTotalLeaveDaysByUserIdAndYear" }
                );
            }
            throw new InvalidOperationException("Repository not initialized.");
        }
    }


}
