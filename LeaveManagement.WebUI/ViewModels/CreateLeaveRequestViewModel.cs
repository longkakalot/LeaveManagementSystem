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

        public string CountryCode { get; set; } = "VN";
        public int ProvinceId { get; set; }
        public int WardId { get; set; }

        public List<SelectListItem> Countries { get; set; } = new();
        public List<SelectListItem> Provinces { get; set; } = new();
        public List<SelectListItem> Wards { get; set; } = new();

        public string? CountryName { get; set; }
        public string? ProvinceName { get; set; }
        public string? WardName { get; set; }

        //public string? SelectedCountryCode { get; set; }
        //public string? SelectedProvinceId { get; set; }
        //public string? SelectedWardId { get; set; }

    }

}
