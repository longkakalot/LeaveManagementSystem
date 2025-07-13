using LeaveManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.GetMyPendingApproval
{
    public class GetMyPendingApprovalsQuery : IRequest<List<LeaveApprovalRequestDto>>
    {
        public int ApproverUserId { get; set; }
    }
}
