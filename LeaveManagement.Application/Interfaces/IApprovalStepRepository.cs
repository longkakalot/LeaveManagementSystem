using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    // Application/Interfaces/IApprovalStepRepository.cs
    public interface IApprovalStepRepository
    {
        Task<List<ApprovalStep>> GetStepsByGroupAsync(string maChucVu, int days);
    }

}
