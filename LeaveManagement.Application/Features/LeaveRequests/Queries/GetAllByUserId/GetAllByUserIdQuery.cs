using LeaveManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.GetAllByUserId
{
    public class GetAllByUserIdQuery : IRequest<List<LeaveRequestDto>> 
    {
        public int UserId { get; set; }
    }

}
