using Dapper;
using LeaveManagement.Application.DTOs;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Infrastructure.Repositories
{   
    public class LeaveApprovalRequestRepository : ILeaveApprovalRequestRepository
    {
        private readonly IDbConnectionFactory? _connectionFactory;
        private readonly IDbConnection? _connection;
        private readonly IDbTransaction? _transaction;

        // Cho Query Handler/DI (chỉ đọc)
        public LeaveApprovalRequestRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Cho UnitOfWork (ghi, transaction)
        public LeaveApprovalRequestRepository(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        /// <summary>
        /// Thêm bản ghi duyệt nghỉ phép, trả về Id vừa tạo. Dùng transaction từ UnitOfWork!
        /// </summary>
        public async Task<int> AddAsync(LeaveApprovalRequest dto)
        {
            if (_connection == null) throw new InvalidOperationException("Repository not initialized for writing.");

            var parameters = new
            {
                dto.LeaveRequestId,
                dto.StepOrder,
                dto.StepApprove,
                dto.ApproverRole,
                dto.ApproverUserIds,
                dto.Status,
                dto.ApprovedAt,
                dto.ApprovedBy,
                dto.Comment,
                dto.CreatedAt,
                Action = "AddNew"
            };

            var newId = await _connection.ExecuteScalarAsync<int>(
                "sp_LeaveApprovalRequest",
                parameters,
                transaction: _transaction, // <-- Truyền transaction ở đây!
                commandType: CommandType.StoredProcedure);

            return newId;
        }

        /// <summary>
        /// Lấy bản ghi LeaveApprovalRequest theo Id
        /// </summary>
        public async Task<LeaveApprovalRequest> GetByIdAsync(int id)
        {
            if (_connection != null)
            {
                var sql = @"SELECT * FROM LeaveApprovalRequests WHERE Id = @Id";
                var result = await _connection.QuerySingleOrDefaultAsync<LeaveApprovalRequest>(
                    sql, new { Id = id }, transaction: _transaction);
                return result!;
            }
            else if (_connectionFactory != null)
            {
                using var connection = _connectionFactory.CreateQueryConnection();
                var sql = @"SELECT * FROM LeaveApprovalRequests WHERE Id = @Id";
                var result = await connection.QuerySingleOrDefaultAsync<LeaveApprovalRequest>(sql, new { Id = id });
                return result!;
            }
            throw new InvalidOperationException("Repository not initialized.");            
        }

        /// <summary>
        /// (Ví dụ) Submit phê duyệt – dùng transaction hiện tại.
        /// </summary>
        //public async Task<int> SubmitLeaveApprovalRequestAsync(LeaveApprovalRequest dto)
        //{
        //    var parameters = new DynamicParameters();
        //    // Ví dụ add các param thực tế ở đây nếu bạn dùng
        //    // parameters.Add("@LeaveRequestId", dto.LeaveRequestId);
        //    // parameters.Add("@ApproverId", ...);
        //    // ...

        //    await _connection.ExecuteAsync(
        //        "sp_SubmitLeaveApprovalRequest",
        //        parameters,
        //        transaction: _transaction, // <-- Cũng truyền transaction ở đây
        //        commandType: CommandType.StoredProcedure);

        //    return (int)dto.Status;
        //}

        /// <summary>
        /// Cập nhật trạng thái và các trường duyệt của LeaveApprovalRequest (gọi khi Approver duyệt phiếu)
        /// </summary>
        public async Task<int> UpdateAsync(LeaveApprovalRequest dto)
        {
            if (_connection == null) throw new InvalidOperationException("Repository not initialized for writing.");
            var sql = "sp_LeaveApprovalRequest";
            var affected = await _connection.ExecuteAsync(
                sql,
                new
                {
                    LeaveApprovalId = dto.Id,
                    StatusApproval = dto.Status,
                    ApprovedBy = dto.ApprovedBy,
                    ApprovedAt = dto.ApprovedAt,
                    Comment = dto.Comment,
                    Action = "UpdateAsync"
                },
                transaction: _transaction);
            return affected;
        }

        /// <summary>
        /// Cập nhật trạng thái về Rejected và các trường duyệt của LeaveApprovalRequest (gọi khi Reject duyệt phiếu)
        /// </summary>
        public async Task<int> RejectAsync(LeaveApprovalRequest dto)
        {
            if (_connection == null) throw new InvalidOperationException("Repository not initialized for writing.");
            var sql = "sp_LeaveApprovalRequest";
            
            var affected = 
             await _connection.ExecuteAsync(
                sql,
                new
                {
                    LeaveApprovalId = dto.Id,
                    StatusApproval = dto.Status, // Enum/string tuỳ cách bạn định nghĩa
                    ApprovedBy = dto.ApprovedBy,
                    ApprovedAt = dto.ApprovedAt,
                    Comment = dto.Comment,
                    Action = "RejectAsync"
                },
                transaction: _transaction);

            return affected;
        }

        public async Task<List<LeaveApprovalRequest>> GetPendingByApproverAsync(int approverUserId)
        {
            if (_connectionFactory != null)
            {
                using var connection = _connectionFactory.CreateCommandConnection();
                var sql = "sp_LeaveApprovalRequest";
                var result = await connection.QueryAsync<LeaveApprovalRequest>(
                    sql,
                    new
                    {
                        Action = "GetPendingByApprover",
                        Status = LeaveApprovalStatus.Pending,
                        ApproverUserId = approverUserId.ToString()
                    });
                return result.ToList();
            }
            else if (_connection != null)
            {
                var sql = "sp_LeaveApprovalRequest";
                var result = await _connection.QueryAsync<LeaveApprovalRequest>(
                    sql,
                    new
                    {
                        Action = "GetPendingByApprover",
                        Status = LeaveApprovalStatus.Pending,
                        ApproverUserId = approverUserId.ToString()
                    },
                    transaction: _transaction);
                return result.ToList();
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        /// <summary>
        /// Cập nhật trạng thái về Canceled và các trường duyệt của LeaveApprovalRequest (gọi khi Cancel duyệt phiếu)
        /// </summary>
        public async Task<int> CancelAsync(LeaveApprovalRequest dto)
        {
            if (_connection == null) throw new InvalidOperationException("Repository not initialized for writing.");
            var sql = "sp_LeaveApprovalRequest";

            var affected =
             await _connection.ExecuteAsync(
                sql,
                new
                {
                    LeaveApprovalId = dto.Id,
                    StatusApproval = dto.Status, // Enum/string tuỳ cách bạn định nghĩa
                    ApprovedBy = dto.ApprovedBy,
                    ApprovedAt = dto.ApprovedAt,
                    Comment = dto.Comment,
                    Action = "CancelAsync"
                },
                transaction: _transaction);

            return affected;
        }

        public async Task<int> UpdateAsyncByStepApprove(LeaveApprovalRequest dto)
        {
            if (_connection == null) throw new InvalidOperationException("Repository not initialized for writing.");
            var sql = "sp_LeaveApprovalRequest";
            var affected = await _connection.ExecuteAsync(
                sql,
                new
                {
                    LeaveRequestId = dto.LeaveRequestId,
                    StatusApproval = dto.Status,
                    StepApprove = dto.StepApprove,                    
                    Action = "UpdateAsyncByStepApprove"
                },
                transaction: _transaction);

            return affected;
        }
    }

}
