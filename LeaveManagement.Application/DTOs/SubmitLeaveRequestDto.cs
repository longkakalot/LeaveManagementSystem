using LeaveManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.DTOs
{
    public class SubmitLeaveRequestDto
    {
        public int LeaveRequestId { get; set; }
        public int ApproverId { get; set; }
        public int Level { get; set; }
        public LeaveApprovalStatus Status { get; set; } = LeaveApprovalStatus.Pending;
    }

}
