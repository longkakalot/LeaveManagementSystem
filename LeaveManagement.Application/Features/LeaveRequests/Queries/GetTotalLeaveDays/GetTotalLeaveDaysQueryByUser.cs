using LeaveManagement.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.GetTotalLeaveDays
{
    public class GetTotalLeaveDaysQueryByUser : IRequest<double>
    {
        public int UserId { get; set; }
        public int Year { get; set; }
        public double TotalLeaveDays { get; set; }
    }    
}
