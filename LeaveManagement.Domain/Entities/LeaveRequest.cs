using LeaveManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Domain.Entities
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? MaChucVu { get; set; }
        public string? MaPhongBan { get; set; }
        public string? TenPhongBan { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? FromDateType { get; set; } // "Full", "Morning", "Afternoon"
        public string? ToDateType { get; set; }   // "Full", "Morning", "Afternoon"
        public double TotalLeaveDays { get; set; }
        public string Reason { get; set; } = default!;
        public string VacationPlace { get; set; } = string.Empty;
        public LeaveStatus Status { get; set; }
        //public string? StatusLabel { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public int StepOrder { get; set; }
        public int StepApprove { get; set; }
        public LeaveApprovalStatus ApproveStatus { get; set; }
                
    }
}
