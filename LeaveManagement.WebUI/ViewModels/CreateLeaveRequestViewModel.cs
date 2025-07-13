using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveManagement.WebUI.ViewModels
{
    public class CreateLeaveRequestViewModel
    {
        public int UserId { get; set; }

        public string? MaChucVu { get; set; }
        public string? MaPhongBan { get; set; }

        public DateTime MocTinhPhep { get; set; }

        public int NgayPhepCongThem { get; set; }
        public int SoNgayNghiCoBan { get; set; }
        public int SoNgayPhepNam { get; set; }        


        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;

        public DateTime FromDate { get; set; }      
        public DateTime ToDate { get; set; }       


        public string? FromDateType { get; set; } // "Full", "Morning", "Afternoon"
        public string? ToDateType { get; set; }   // "Full", "Morning", "Afternoon"

        public double TotalLeaveDays { get; set; }
        public string Reason { get; set; } = string.Empty;

        public string VacationPlace { get; set; } = string.Empty;
        public int Status { get; set; }
        public List<SelectListItem> LeaveTypes { get; set; } = new();
    }

}
