using Azure.Core;
using Dapper;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Infrastructure.Repositories
{
    public class LeaveRequestDetailRepository : ILeaveRequestDetailRepository
    {
        private readonly IDbConnectionFactory? _connectionFactory;
        private readonly IDbConnection? _connection;
        private readonly IDbTransaction? _transaction;


        // Mode 1: Query (inject vào DI cho query handler, không dùng transaction)
        public LeaveRequestDetailRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Nhận connection và transaction từ UnitOfWork
        public LeaveRequestDetailRepository(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> AddRangeAsync(IEnumerable<LeaveRequestDetail> details)
        {
            #region Cách 1
            //-Cách 1: Insert từng dòng-//
            //int count = 0;
            //foreach (var item in details)
            //{
            //    var parameters = new DynamicParameters();
            //    parameters.Add("@LeaveRequestId", item.LeaveRequestId);
            //    parameters.Add("@Date", item.Date);
            //    parameters.Add("@Period", item.Period);
            //    parameters.Add("@YearDetail", item.Year);
            //    parameters.Add("@Value", item.Value);
            //    parameters.Add("@Action", "AddNewDetail");
            //    parameters.Add("@Id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            //    var sql = "sp_LEAVEREQUESTS";
            //    if (_connection != null)
            //    {
            //        await _connection.ExecuteAsync(
            //            sql,
            //            parameters,
            //            transaction: _transaction,
            //            commandType: CommandType.StoredProcedure
            //        );
            //    }
            //    else if (_connectionFactory != null)
            //    {
            //        using var conn = _connectionFactory.CreateQueryConnection();
            //        await conn.ExecuteAsync(
            //            sql,
            //            parameters,
            //            transaction: _transaction,
            //            commandType: CommandType.StoredProcedure
            //        );
            //    }
            //    else
            //    {
            //        throw new InvalidOperationException("Repository not initialized.");
            //    }

            //    count++;
            //    // Nếu bạn muốn lấy tất cả Id vừa insert, có thể tạo 1 list để lưu lại Ids
            //    // int insertedId = parameters.Get<int>("@Id");
            //    // ids.Add(insertedId);
            //}
            //return count; // Tổng số dòng đã insert
            #endregion End Cách 1

            //-Cách 2: truyền list sang stored-//
            var dt = new DataTable();
            dt.Columns.Add("LeaveRequestId", typeof(int));
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("Period", typeof(string));
            dt.Columns.Add("Year", typeof(int));
            dt.Columns.Add("Value", typeof(decimal));

            foreach (var item in details)
            {
                dt.Rows.Add(item.LeaveRequestId, item.Date, item.Period, item.Year, item.Value);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@Details", dt.AsTableValuedParameter("LeaveRequestDetailType"));
            parameters.Add("@Action", "AddNewDetailBulk");

            var sql = "sp_LEAVEREQUESTS";
            int affected = 0;

            if (_connection != null)
            {
                affected = await _connection.ExecuteAsync(
                    sql,
                    parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure
                );
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateCommandConnection();
                affected = await conn.ExecuteAsync(
                    sql,
                    parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure
                );
            }
            else
            {
                throw new InvalidOperationException("Repository not initialized.");
            }

            return affected; // Số dòng đã insert thành công
        }      

        public async Task<bool> DeleteByLeaveRequestId(int leaveRequestId)
        {
            var sql = "sp_LEAVEREQUESTS";
            if (_connection != null)
            {
                var affectedRows = await _connection.ExecuteAsync(
                    sql,
                    new { LeaveRequestId = leaveRequestId, Action = "DeleteDetail" },
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure);

                return affectedRows > 0;
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var affectedRows = await conn.ExecuteAsync(
                    sql,
                    new { LeaveRequestId = leaveRequestId, Action = "DeleteDetail" },
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure);

                return affectedRows > 0;
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public Task<List<LeaveRequestDetail>> GetByDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public async Task<List<LeaveRequestDetail>> GetByLeaveRequestId(int leaveRequestId)
        {
            var sql = "sp_LEAVEREQUESTS";
            if (_connection != null)
            {
                var result = await _connection.QueryAsync<LeaveRequestDetail>(
                    sql,
                    new { LeaveRequestId = leaveRequestId, Action = "GetByLeaveRequestId" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var result = await conn.QueryAsync<LeaveRequestDetail>(
                    sql,
                    new { LeaveRequestId = leaveRequestId, Action = "GetByLeaveRequestId" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public Task<List<LeaveRequestDetail>> GetByUserId(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
