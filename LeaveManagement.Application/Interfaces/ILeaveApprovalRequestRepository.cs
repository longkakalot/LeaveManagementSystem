using LeaveManagement.Application.DTOs;
using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    public interface ILeaveApprovalRequestRepository
    {
        //Task<int> SubmitLeaveApprovalRequestAsync(LeaveApprovalRequest dto);
        Task<int> AddAsync (LeaveApprovalRequest dto);
        Task<LeaveApprovalRequest> GetByIdAsync (int id);
        Task<int> UpdateAsync(LeaveApprovalRequest dto);
        Task<int> UpdateAsyncByStepApprove(LeaveApprovalRequest dto);
        Task<int> RejectAsync(LeaveApprovalRequest dto);
        Task<int> CancelAsync(LeaveApprovalRequest dto);
        Task<List<LeaveApprovalRequest>> GetPendingByApproverAsync(int approverUserId);
    }
}
