using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.DTOs
{
    public class UserLeaveBalancesDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Year { get; set; }
        public double LeaveDaysGranted { get; set; }
        public double LeaveDaysTaken { get; set; } //số ngày đã nghỉ
        public double LeaveDaysRemain { get; set; } // số ngày còn lại
        public double DaysToDeduct { get; set; }
        public double DaysToReturn { get; set; }
    }
}
