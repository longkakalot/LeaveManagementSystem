using LeaveManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.GetLeaveRequestById
{   

    public class GetLeaveRequestByIdQuery : IRequest<LeaveRequestDto>
    {
        public int Id { get; set; }
    }
}
