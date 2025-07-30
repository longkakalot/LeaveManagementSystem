using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Domain.Entities
{
    public class UserLeaveBalances
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Year { get; set; }
        public double LeaveDaysGranted { get; set; }
        public double LeaveDaysTaken { get; set; }
        public double LeaveDaysRemain { get; set; }
        public double DaysToDeduct { get; set; }
        public double DaysToReturn { get; set; }
    }
}
