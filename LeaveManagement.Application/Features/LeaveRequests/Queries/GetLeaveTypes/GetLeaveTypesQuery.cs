using LeaveManagement.Application.DTOs;
using LeaveManagement.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.GetLeaveTypes
{
    public class GetLeaveTypesQuery : IRequest<List<LeaveType>>
    {
    }
}
