using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    public interface ILeaveRequestRepository
    {
        Task<int> CreateAsync(LeaveRequest request);
        Task<LeaveRequest> GetByIdAsync(int id);
        Task<bool> UpdateAsync(LeaveRequest request);
        Task<bool> UpdateStatusAsync(LeaveRequest request);
        Task<bool> DeleteAsync(int id);
        Task<double> GetTotalLeaveDaysAsync(int userId, int year);

    }
}
