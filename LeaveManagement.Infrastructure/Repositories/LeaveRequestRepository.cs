using Dapper;
using LeaveManagement.Application.DTOs;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Infrastructure.Configuration;
using LeaveManagement.Infrastructure.Persistence.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Infrastructure.Repositories
{
    //public class LeaveRequestRepository : ILeaveRequestRepository
    //{
    //    private readonly IDbConnectionFactory? _connectionFactory;
    //    private readonly IDbConnection? _connection;
    //    private readonly IDbTransaction? _transaction;


    //    // Mode 1: Query (inject vào DI cho query handler, không dùng transaction)
    //    public LeaveRequestRepository(IDbConnectionFactory connectionFactory)
    //    {
    //        _connectionFactory = connectionFactory;
    //    }

    //    // Nhận connection và transaction từ UnitOfWork
    //    public LeaveRequestRepository(IDbConnection connection, IDbTransaction transaction)
    //    {
    //        _connection = connection;
    //        _transaction = transaction;
    //    }

    //    public async Task<int> CreateAsync(LeaveRequest request)
    //    {
    //        var parameters = new DynamicParameters();
    //        parameters.Add("@UserId", request.UserId);
    //        parameters.Add("@FullName", request.FullName);
    //        parameters.Add("@MaChucVu", request.MaChucVu);
    //        parameters.Add("@MaPhongBan", request.MaPhongBan);
    //        parameters.Add("@TenPhongBan", request.TenPhongBan);
    //        parameters.Add("@LeaveTypeId", request.LeaveTypeId);
    //        parameters.Add("@FromDate", request.FromDate);
    //        parameters.Add("@ToDate", request.ToDate);
    //        parameters.Add("@FromDateType", request.FromDateType);
    //        parameters.Add("@ToDateType", request.ToDateType);
    //        parameters.Add("@TotalLeaveDays", request.TotalLeaveDays);
    //        parameters.Add("@Reason", request.Reason);
    //        parameters.Add("@VacationPlace", request.VacationPlace);
    //        parameters.Add("@Action", "AddNew");
    //        parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

    //        var sql = "sp_LEAVEREQUESTS";

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

    //    public async Task<bool> DeleteAsync(int id)
    //    {
    //        var sql = "sp_LEAVEREQUESTS";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateCommandConnection();
    //        var affectedRows = await conn.ExecuteAsync(
    //            sql,
    //            new { Id = id, Action = "Delete" },
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return affectedRows > 0;
    //    }

    //    public async Task<List<DateTime>> GetAllCompensateDayAsync(DateTime from, DateTime to)
    //    {
    //        var sql = "sp_LEAVEREQUESTS";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateQueryConnection();
    //        var result = await conn.QueryAsync<DateTime>(
    //            sql,
    //            new { FromDate = from, ToDate = to, Action = "GetCompensatoryDayInrange" },
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return result.ToList();
    //    }

    //    public async Task<List<DateTime>> GetAllHolidaysAsync(DateTime from, DateTime to)
    //    {
    //        var sql = "sp_LEAVEREQUESTS";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateQueryConnection();
    //        var result = await conn.QueryAsync<DateTime>(
    //            sql,
    //            new { FromDate = from, ToDate = to, Action = "GetHolidaysInrange" },
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return result.ToList();
    //    }

    //    public async Task<LeaveRequest> GetByIdAsync(int id)
    //    {
    //        var sql = "sp_LEAVEREQUESTS";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateQueryConnection();
    //        var result = await conn.QuerySingleOrDefaultAsync<LeaveRequest>(
    //            sql,
    //            new { Id = id, Action = "GetData_ByKey" },
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return result!;
    //    }

    //    public async Task<double> GetTotalLeaveDaysAsync(int userId, int year)
    //    {
    //        var sql = "sp_LEAVEREQUESTS";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateQueryConnection();
    //        var result = await conn.QuerySingleOrDefaultAsync<double>(
    //            sql,
    //            new { UserId = userId, Year = year, Action = "GetTotalLeaveDaysAsync" },
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return result;
    //    }

    //    public async Task<bool> UpdateAsync(LeaveRequest leaveRequest)
    //    {
    //        var sql = "sp_LEAVEREQUESTS";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateCommandConnection();
    //        var affectedRows = await conn.ExecuteAsync(
    //            sql,
    //            new
    //            {
    //                UserId = leaveRequest.UserId,
    //                Id = leaveRequest.Id,
    //                LeaveTypeId = leaveRequest.LeaveTypeId,
    //                FromDate = leaveRequest.FromDate,
    //                FromDateType = leaveRequest.FromDateType,
    //                ToDateType = leaveRequest.ToDateType,
    //                ToDate = leaveRequest.ToDate,
    //                TotalLeaveDays = leaveRequest.TotalLeaveDays,
    //                Reason = leaveRequest.Reason,
    //                VacationPlace = leaveRequest.VacationPlace,
    //                Action = "Update"
    //            },
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return affectedRows > 0;
    //    }

    //    public async Task<bool> UpdateStatusAsync(LeaveRequest leaveRequest)
    //    {
    //        var sql = "sp_LEAVEREQUESTS";

    //        if (_connectionFactory == null)
    //            throw new InvalidOperationException("Repository not initialized with ConnectionFactory.");

    //        using var conn = _connectionFactory.CreateCommandConnection();
    //        var affectedRows = await conn.ExecuteAsync(
    //            sql,
    //            new
    //            {
    //                UserId = leaveRequest.UserId,
    //                Id = leaveRequest.Id,
    //                TotalLeaveDays = leaveRequest.TotalLeaveDays,
    //                Status = leaveRequest.Status,
    //                Action = "UpdateStatus"
    //            },
    //            commandType: CommandType.StoredProcedure
    //        );
    //        return affectedRows > 0;
    //    }
    //}



    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly IDbConnectionFactory? _connectionFactory;
        private readonly IDbConnection? _connection;
        private readonly IDbTransaction? _transaction;


        // Mode 1: Query (inject vào DI cho query handler, không dùng transaction)
        public LeaveRequestRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Nhận connection và transaction từ UnitOfWork
        public LeaveRequestRepository(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateAsync(LeaveRequest request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", request.UserId);
            parameters.Add("@FullName", request.FullName);
            parameters.Add("@MaChucVu", request.MaChucVu);
            parameters.Add("@MaPhongBan", request.MaPhongBan);
            parameters.Add("@TenPhongBan", request.TenPhongBan);
            parameters.Add("@LeaveTypeId", request.LeaveTypeId);
            parameters.Add("@FromDate", request.FromDate);
            parameters.Add("@ToDate", request.ToDate);
            parameters.Add("@FromDateType", request.FromDateType);
            parameters.Add("@ToDateType", request.ToDateType);
            parameters.Add("@TotalLeaveDays", request.TotalLeaveDays);
            parameters.Add("@Reason", request.Reason);
            parameters.Add("@VacationPlace", request.VacationPlace);
            parameters.Add("@Action", "AddNew");
            parameters.Add("@Id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            var sql = "sp_LEAVEREQUESTS";
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




            //await _connection.ExecuteAsync(
            //    "sp_LEAVEREQUESTS",
            //    parameters,
            //    transaction: _transaction, // truyền transaction
            //    commandType: CommandType.StoredProcedure
            //);

            //return parameters.Get<int>("@Id");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "sp_LEAVEREQUESTS";
            if (_connection != null)
            {
                var affectedRows = await _connection.ExecuteAsync(
                    sql,
                    new { Id = id, Action = "Delete" },
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure);

                return affectedRows > 0;
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var affectedRows = await conn.ExecuteAsync(
                    sql,
                    new { Id = id, Action = "Delete" },
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure);

                return affectedRows > 0;
            }
            throw new InvalidOperationException("Repository not initialized.");





            //var affectedRows = await _connection.ExecuteAsync(
            //    "sp_LEAVEREQUESTS",
            //    new { Id = id, Action = "Delete" },
            //    transaction: _transaction,
            //    commandType: CommandType.StoredProcedure);

            //return affectedRows > 0;
        }

        public async Task<List<LeaveRequest>> GetAllByUserId(int userId)
        {
            var sql = "sp_LEAVEREQUESTS";
            if (_connection != null)
            {
                var result = await _connection.QueryAsync<LeaveRequest>(
                    sql,
                    new { UserId = userId, Action = "GetAllByUserId" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var result = await conn.QueryAsync<LeaveRequest>(
                    sql,
                    new { UserId = userId, Action = "GetAllByUserId" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public async Task<List<DateTime>> GetAllCompensateDayAsync(DateTime from, DateTime to)
        {
            var sql = "sp_LEAVEREQUESTS";
            if (_connection != null)
            {
                var result = await _connection.QueryAsync<DateTime>(
                    sql,
                    new { FromDate = from, ToDate = to, Action = "GetCompensatoryDayInrange" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var result = await conn.QueryAsync<DateTime>(
                    sql,
                    new { FromDate = from, ToDate = to, Action = "GetCompensatoryDayInrange" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public async Task<List<DateTime>> GetAllHolidaysAsync(DateTime from, DateTime to)
        {
            var sql = "sp_LEAVEREQUESTS";
            if (_connection != null)
            {
                var result = await _connection.QueryAsync<DateTime>(
                    sql,
                    new { FromDate = from, ToDate = to, Action = "GetHolidaysInrange" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var result = await conn.QueryAsync<DateTime>(
                    sql,
                    new { FromDate = from, ToDate = to, Action = "GetHolidaysInrange" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public async Task<LeaveRequest> GetByIdAsync(int id)
        {
            var sql = "sp_LEAVEREQUESTS";
            if (_connection != null)
            {
                var result = await _connection.QuerySingleOrDefaultAsync<LeaveRequest>(
                    sql,
                    new { Id = id, Action = "GetData_ByKey" },
                    transaction: _transaction
                );
                return result!;
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var result = await conn.QuerySingleOrDefaultAsync<LeaveRequest>(
                    sql,
                    new { Id = id, Action = "GetData_ByKey" },
                    transaction: _transaction
                );
                return result!;
            }
            throw new InvalidOperationException("Repository not initialized.");



            //// Nếu bạn muốn lấy dữ liệu ngoài transaction (chỉ đọc), 
            //// bạn có thể truyền null, hoặc overload repo cho trường hợp chỉ đọc.
            //var result = await _connection.QuerySingleOrDefaultAsync<LeaveRequest>(
            //    "sp_LEAVEREQUESTS",
            //    new { Id = id, Action = "GetData_ByKey" },
            //    transaction: _transaction,
            //    commandType: CommandType.StoredProcedure);

            //return result!;
        }

        public async Task<double> GetTotalLeaveDaysAsync(int userId, int year)
        {
            var sql = "sp_LEAVEREQUESTS";
            if (_connection != null)
            {
                var result = await _connection.QuerySingleOrDefaultAsync<double>(
                    sql,
                    new { UserId = userId, Year = year, Action = "GetTotalLeaveDaysAsync" },
                    transaction: _transaction
                );
                return result;
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var result = await conn.QuerySingleOrDefaultAsync<double>(
                    sql,
                    new { UserId = userId, Year = year, Action = "GetTotalLeaveDaysAsync" },
                    transaction: _transaction
                );
                return result;
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public async Task<bool> UpdateAsync(LeaveRequest leaveRequest)
        {
            var sql = "sp_LEAVEREQUESTS";
            if (_connection != null)
            {
                var affectedRows = await _connection.ExecuteAsync(
                    sql,
                    new
                    {
                        UserId = leaveRequest.UserId,
                        Id = leaveRequest.Id,
                        LeaveTypeId = leaveRequest.LeaveTypeId,
                        FromDate = leaveRequest.FromDate,
                        FromDateType = leaveRequest.FromDateType,
                        ToDateType = leaveRequest.ToDateType,
                        ToDate = leaveRequest.ToDate,
                        TotalLeaveDays = leaveRequest.TotalLeaveDays,
                        Reason = leaveRequest.Reason,
                        VacationPlace = leaveRequest.VacationPlace,
                        Action = "Update"
                    },
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure);

                return affectedRows > 0;
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var affectedRows = await conn.ExecuteAsync(
                    sql,
                    new
                    {
                        UserId = leaveRequest.UserId,
                        Id = leaveRequest.Id,
                        LeaveTypeId = leaveRequest.LeaveTypeId,
                        FromDate = leaveRequest.FromDate,
                        FromDateType = leaveRequest.FromDateType,
                        ToDateType = leaveRequest.ToDateType,
                        ToDate = leaveRequest.ToDate,
                        TotalLeaveDays = leaveRequest.TotalLeaveDays,
                        Reason = leaveRequest.Reason,
                        VacationPlace = leaveRequest.VacationPlace,
                        Action = "Update"
                    },
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure);

                return affectedRows > 0;
            }
            throw new InvalidOperationException("Repository not initialized.");





            //var affectedRows = await _connection.ExecuteAsync(
            //    "sp_LEAVEREQUESTS",
            //    new
            //    {
            //        UserId = leaveRequest.UserId,
            //        Id = leaveRequest.Id,
            //        LeaveTypeId = leaveRequest.LeaveTypeId,
            //        FromDate = leaveRequest.FromDate,
            //        FromDateType = leaveRequest.FromDateType,
            //        ToDateType = leaveRequest.ToDateType,
            //        ToDate = leaveRequest.ToDate,
            //        TotalLeaveDays = leaveRequest.TotalLeaveDays,
            //        Reason = leaveRequest.Reason,
            //        VacationPlace = leaveRequest.VacationPlace,
            //        Action = "Update"
            //    },
            //    transaction: _transaction,
            //    commandType: CommandType.StoredProcedure);

            //return affectedRows > 0;
        }

        public async Task<bool> UpdateStatusAsync(LeaveRequest leaveRequest)
        {
            var sql = "sp_LEAVEREQUESTS";
            if (_connection != null)
            {
                var affectedRows = await _connection.ExecuteAsync(
                    sql,
                    new
                    {
                        UserId = leaveRequest.UserId,
                        Id = leaveRequest.Id,
                        TotalLeaveDays = leaveRequest.TotalLeaveDays,
                        Status = leaveRequest.Status,
                        Action = "UpdateStatus"
                    },
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure);

                return affectedRows > 0;
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var affectedRows = await conn.ExecuteAsync(
                    sql,
                    new
                    {
                        UserId = leaveRequest.UserId,
                        Id = leaveRequest.Id,
                        TotalLeaveDays = leaveRequest.TotalLeaveDays,
                        Status = leaveRequest.Status,
                        Action = "UpdateStatus"
                    },
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure);

                return affectedRows > 0;
            }
            throw new InvalidOperationException("Repository not initialized.");



            //var affectedRows = await _connection.ExecuteAsync(
            //    "sp_LEAVEREQUESTS",
            //    new
            //    {
            //        UserId = leaveRequest.UserId,
            //        Id = leaveRequest.Id,
            //        TotalLeaveDays = leaveRequest.TotalLeaveDays,
            //        Status = leaveRequest.Status,
            //        Action = "UpdateStatus"
            //    },
            //    transaction: _transaction,
            //    commandType: CommandType.StoredProcedure);

            //return affectedRows > 0;
        }
    }

}
