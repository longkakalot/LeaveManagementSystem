using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    public interface IUserLeaveBalanceRepository
    {
        Task<int> AddUserLeaveBalance(UserLeaveBalances dto);
        Task<bool> UpdateUserLeaveBalance(UserLeaveBalances dto);
        Task<bool> DeductUserLeaveBalance(UserLeaveBalances dto); 
        Task<bool> ReturnUserLeaveBalance(UserLeaveBalances dto);
        Task<UserLeaveBalances> GetUserLeaveBalance(int userId, int year); 
    }
}
