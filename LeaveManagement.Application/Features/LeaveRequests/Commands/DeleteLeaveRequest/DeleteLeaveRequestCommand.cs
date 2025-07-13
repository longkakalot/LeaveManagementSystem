using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.DeleteLeaveRequest
{
    public class DeleteLeaveRequestCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

}
