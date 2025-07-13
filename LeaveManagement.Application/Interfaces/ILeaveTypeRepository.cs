using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    public interface ILeaveTypeRepository
    {
        Task<int> CreateAsync(LeaveType request);
        Task<List<LeaveType>> GetAllAsync();
    }
}
