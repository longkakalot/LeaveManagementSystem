using LeaveManagement.Application.Authentication.Dtos;
using LeaveManagement.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.GetTotalLeaveDaysByUserIdAndYear
{
    public class GetTotalLeaveDaysByUserIdAndYear : IRequest<UserDto>
    {
        public int UserId { get; set; }
        public int Year { get; set; }
        public double TotalLeaveDays { get; set; }
    }    
}
