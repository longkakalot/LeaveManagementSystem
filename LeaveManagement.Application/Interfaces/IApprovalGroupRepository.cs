using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    public interface IApprovalGroupRepository
    {
        Task<List<ApprovalGroup>> GetByRoleAndConditionAsync(string maChucVu, int days);
    }
}
