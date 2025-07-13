using LeaveManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Domain.Entities
{
    // Domain/Entities/LeaveApprovalRequest.cs
    public class LeaveApprovalRequest
    {

        public int Id { get; set; }
        public int LeaveRequestId { get; set; }
        public int StepOrder { get; set; }
        public int StepApprove { get; set; }
        public string? ApproverRole { get; set; }
        public string? ApproverUserIds { get; set; }
        public LeaveApprovalStatus Status { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int ApprovedBy { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public string RequesterName { get; set; } = "";


        public string? MaChucVu { get; set; }
        public string? MaPhongBan { get; set; }
        public string? TenPhongBan { get; set; }
        public string VacationPlace { get; set; } = string.Empty;
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public double TotalLeaveDays { get; set; }
    }



    //public class LeaveApprovalRequest
    //{
    //    public int Id { get; set; }
    //    public int LeaveRequestId { get; set; }
    //    public int ApproverId { get; set; }
    //    public int Level { get; set; }
    //    public LeaveApprovalStatus Status { get; set; } // Enum: Pending, Approved, Rejected
    //    public string? Comments { get; set; }
    //    public DateTime CreatedAt { get; set; }
    //}

}
