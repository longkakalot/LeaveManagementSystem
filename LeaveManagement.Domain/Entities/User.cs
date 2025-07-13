using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public bool IsActive { get; set; } = true;

        public string? MaChucVu { get; set; }
        public string? MaPhongBan { get; set; }
        public string? TenPhongBan { get; set; }
        public DateTime MocTinhPhep { get; set; }

        public int NgayPhepCongThem { get; set; }
        public int SoNgayNghiCoBan { get; set; }
        public int SoNgayPhepNam { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
