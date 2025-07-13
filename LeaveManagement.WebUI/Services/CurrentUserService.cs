using LeaveManagement.Application.Interfaces;
using System.Security.Claims;

namespace LeaveManagement.WebUI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal? _user;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _user = httpContextAccessor.HttpContext?.User;
        }

        public int Id =>
        int.TryParse(_user?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        public int UserId =>
        int.TryParse(_user?.FindFirstValue(ClaimTypes.NameIdentifier), out var UserId) ? UserId : 0;

        public string Username => _user?.Identity?.Name ?? "";

        public string FullName => _user?.FindFirstValue("FullName") ?? "";

        public string MaChucVu => _user?.FindFirstValue("MaChucVu") ?? "";

        public string MaPhongBan => _user?.FindFirstValue("MaPhongBan") ?? "";
        public string TenPhongBan => _user?.FindFirstValue("TenPhongBan") ?? "";

        public int SoNgayPhepNam =>
    int.TryParse(_user?.FindFirstValue("SoNgayPhepNam"), out var soNgayPhepNamValue) ? soNgayPhepNamValue : 0;

        public int NgayPhepCongThem =>
            int.TryParse(_user?.FindFirstValue("NgayPhepCongThem"), out var ngayPhepCongThemValue) ? ngayPhepCongThemValue : 0;

        public int SoNgayNghiCoBan =>
            int.TryParse(_user?.FindFirstValue("SoNgayNghiCoBan"), out var soNgayNghiCoBanValue) ? soNgayNghiCoBanValue : 0;



    }
}
