using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    public interface ICurrentUserService
    {
        int Id { get; }

        int UserId { get; }
        string? Username { get; }
        string FullName { get; }
        //string Email { get;  }
        //string PasswordHash { get; }
        //bool IsActive { get; } = true;
        string? MaChucVu { get; }
        string? MaPhongBan { get; }
        string? TenPhongBan { get; }

        //public DateTime MocTinhPhep { get; set; }

        int NgayPhepCongThem { get; }
        int SoNgayNghiCoBan { get; }
        int SoNgayPhepNam { get; }
        //string? Role { get; }
    }
}
