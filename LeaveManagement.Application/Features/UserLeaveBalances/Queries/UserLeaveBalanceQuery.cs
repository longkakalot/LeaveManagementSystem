using LeaveManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.UserLeaveBalances.Queries
{
    public class UserLeaveBalanceQuery : IRequest<UserLeaveBalancesDto>
    {
        public int UserId { get; set; }
        public int Year { get; set; }
    }
}
