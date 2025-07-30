using LeaveManagement.Application.Common;
using LeaveManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.CreateLeaveRequest
{
    public class CreateLeaveRequestCommand : IRequest<ServiceResult>
    {
        public int LeaveRequestId { get; set; }
        
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? MaChucVu { get; set; }
        public string? MaPhongBan { get; set; }
        public string? TenPhongBan { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? FromDateType { get; set; }
        public string? ToDateType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string VacationPlace { get; set; } = string.Empty;
        public int NgayPhepCongThem { get; set; }
        public int SoNgayNghiCoBan { get; set; }
        public int SoNgayPhepNam { get; set; }
        public int SoNgayPhepNamMoi { get; set; }
        public double TotalLeaveDays { get; set; }

    }
}
