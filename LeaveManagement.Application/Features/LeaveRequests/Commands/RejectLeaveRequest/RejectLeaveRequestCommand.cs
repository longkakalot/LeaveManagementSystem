using LeaveManagement.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.RejectLeaveRequest
{
    public class RejectLeaveRequestCommand : IRequest<ServiceResult>
    {
        public int LeaveApprovalRequestId { get; set; }
        public int ApproverId { get; set; }
        public string? Comment { get; set; }
    }

}
