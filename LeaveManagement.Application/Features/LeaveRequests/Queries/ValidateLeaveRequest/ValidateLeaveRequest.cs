using LeaveManagement.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.ValidateLeaveRequest
{
    public class ValidateLeaveRequest : IRequest<ServiceResult>
    {
        public int UserId { get; set; }
        public DateTime FromDate { get; set; }
        public string? FromDateType { get; set; }
        public DateTime ToDate { get; set; }
        public string? ToDateType { get; set; }
        public int LeaveTypeId { get; set; }
        // ... các trường khác nếu cần
    }

}
