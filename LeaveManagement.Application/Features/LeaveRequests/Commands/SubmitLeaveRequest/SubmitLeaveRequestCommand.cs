using LeaveManagement.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.SubmitLeaveRequest
{
    public class SubmitLeaveRequestCommand : IRequest<ServiceResult>
    {
        public int LeaveRequestId { get; set; }
        public int UserId { get; set; }
    }

    //public class SubmitLeaveRequestCommand : IRequest<int>
    //{
    //    public int LeaveRequestId { get; set; }
    //    public int SoNgayNghi { get; set; }
    //    public int ApproverId { get; set; } // người sẽ duyệt

    //    public DateTime FromDate { get; set; }
    //    public DateTime ToDate { get; set; }
    //    public string? Reason { get; set; }
    //    public int Status { get; set; }
    //}
}
