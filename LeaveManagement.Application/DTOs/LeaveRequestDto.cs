using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.DTOs
{
    public class LeaveRequestDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? MaChucVu { get; set; }
        public string? MaPhongBan { get; set; }
        public string? TenPhongBan { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? FromDateType { get; set; } // "Full", "Morning", "Afternoon"
        public string? ToDateType { get; set; }   // "Full", "Morning", "Afternoon"
        public double TotalLeaveDays { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string VacationPlace { get; set; } = string.Empty;
        public LeaveStatus Status { get; set; }
        public string? StatusLabel { get; set; }
        public int StepOrder { get; set; }
        public int StepApprove { get; set; }
        public LeaveApprovalStatus ApproveStatus { get; set; }
        public List<LeaveRequestDetailDto> Details { get; set; } = new();
        public string? CountryName { get; set; }
        public string? ProvinceName { get; set; }
        public string? WardName { get; set; }
    }
    public class LeaveRequestDetailDto
    {
        public DateTime Date { get; set; }
        public string? Period { get; set; } // "FullDay", "Morning", "Afternoon"
        public double Value { get; set; }  // 1.0, 0.5
        public int Year { get; set; }
    }
}
