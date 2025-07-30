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
    //public class UnitOfWork : IUnitOfWork
    //{
    //    public ILeaveRequestRepository LeaveRequests { get; }

    //    public UnitOfWork(ILeaveRequestRepository leaveRequestRepository)
    //    {
    //        LeaveRequests = leaveRequestRepository;
    //    }

    //    public Task CommitAsync() => Task.CompletedTask;
    //}

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public IApprovalGroupRepository ApprovalGroups { get; }
        public IApprovalStepRepository ApprovalSteps { get; }
        public ILeaveApprovalRequestRepository LeaveApprovalRequests { get; }
        public ILeaveRequestRepository LeaveRequests { get; }
        public ILeaveTypeRepository LeaveTypes { get; }
        public IUserRepository Users { get; }
        public IHolidayRepository Holidays { get; }
        public ICompensateWorkingDayRepository CompensateWorkingDays { get; }
        public IUserLeaveBalanceRepository UserLeaveBalances { get; }
        public ILeaveRequestDetailRepository LeaveRequestDetails { get; }
        public IApproverService ApproverService => throw new NotImplementedException();

        // ... các repo khác

        public UnitOfWork(IDbConnectionFactory connectionFactory)
        {
            // Luôn mở 1 connection duy nhất cho transaction này
            _connection = connectionFactory.CreateCommandConnection();
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            // Tạo transaction thực sự
            _transaction = _connection.BeginTransaction();

            // Truyền connection & transaction vào các repo
            ApprovalGroups = new ApprovalGroupRepository(_connection, _transaction);
            ApprovalSteps = new ApprovalStepRepository(_connection, _transaction);
            LeaveApprovalRequests = new LeaveApprovalRequestRepository(_connection, _transaction);
            LeaveRequests = new LeaveRequestRepository(_connection, _transaction);
            LeaveTypes = new LeaveTypeRepository(_connection, _transaction);
            Users = new UserRepository(_connection, _transaction);
            Holidays = new HolidayRepository(_connection, _transaction);
            UserLeaveBalances = new UserLeaveBalanceRepository(_connection, _transaction);
            CompensateWorkingDays = new CompensateWorkingDayRepository(_connection, _transaction);
            LeaveRequestDetails = new LeaveRequestDetailRepository(_connection, _transaction);
            // ... các repo khác
        }

        /// <summary>
        /// Ghi nhận tất cả thay đổi. Nếu có exception ở bất kỳ đâu trước khi gọi Commit thì transaction sẽ rollback khi Dispose (do using).
        /// </summary>
        public void Commit()
        {
            _transaction?.Commit();
            Dispose();
        }

        /// <summary>
        /// Rollback tất cả các thay đổi nếu có lỗi ở bất kỳ thao tác nào.
        /// </summary>
        public void Rollback()
        {
            _transaction?.Rollback();
            Dispose();
        }

        /// <summary>
        /// Đảm bảo giải phóng tài nguyên connection/transaction sau khi commit hoặc rollback.
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            if (_connection?.State == ConnectionState.Open)
                _connection.Dispose();
        }
    }
}
