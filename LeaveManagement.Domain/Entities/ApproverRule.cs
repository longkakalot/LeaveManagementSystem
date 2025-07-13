using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Domain.Entities
{
    public class ApproverRule
    {
        public int Id { get; set; } 
        public string? RoleNguoiGui { get; set; }
        public int ApproverOrder { get; set; }
        public string? ApproverRole { get; set; }
        public int NumberApprove { get; set; }

    }
}