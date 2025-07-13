using LeaveManagement.Domain.Enums;

namespace LeaveManagement.WebUI.ViewModels
{
    public class LeaveApprovalRequestDto
    {
        public int Id { get; set; }
        public int LeaveRequestId { get; set; }
        public string RequesterName { get; set; } = "";
        public int StepOrder { get; set; }
        public int StepApprove { get; set; }
        public string ApproverRole { get; set; } = "";
        public LeaveApprovalStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Comment { get; set; } = string.Empty;


        public string? MaChucVu { get; set; }
        public string? MaPhongBan { get; set; }
        public string? TenPhongBan { get; set; }
        public string VacationPlace { get; set; } = string.Empty;
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double TotalLeaveDays { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

}
