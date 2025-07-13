using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Domain.Entities
{
    // Domain/Entities/ApprovalStep.cs
    public class ApprovalStep
    {
        public int Id { get; set; }
        public string? GroupName { get; set; }
        public int StepOrder { get; set; }
        public string? ApproverRole { get; set; }
        public bool? OnlyOver5Days { get; set; }
    }

}
