using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    public interface IApproverService
    {
        Task<int> GetApproverIdAsync(int employeeId, int level);
    }
}
