using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.EditLeaveRequest
{
    public class EditLeaveRequestCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? FromDateType { get; set; } // "Full", "Morning", "Afternoon"
        public string? ToDateType { get; set; }   // "Full", "Morning", "Afternoon"
        public string? Reason { get; set; }
        public string? VacationPlace { get; set; }
    }

}
