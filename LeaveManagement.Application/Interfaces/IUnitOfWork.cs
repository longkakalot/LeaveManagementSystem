using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    //public interface IUnitOfWork
    //{
    //    ILeaveRequestRepository LeaveRequests { get; }
    //    Task CommitAsync();
    //}

    public interface IUnitOfWork : IDisposable
    {
        IApprovalGroupRepository ApprovalGroups { get; }
        IApprovalStepRepository ApprovalSteps { get; }
        IApproverService ApproverService { get; }        
        ILeaveApprovalRequestRepository LeaveApprovalRequests { get; }
        ILeaveRequestRepository LeaveRequests { get; }
        ILeaveTypeRepository LeaveTypes { get; }
        IUserRepository Users { get; }
        IHolidayRepository Holidays { get; }
        ICompensateWorkingDayRepository CompensateWorkingDays { get; }
        IUserLeaveBalanceRepository UserLeaveBalances { get; }
        ILeaveRequestDetailRepository LeaveRequestDetails { get; }
        // ... các repo khác nếu có

        void Commit();
        void Rollback();
    }

}
