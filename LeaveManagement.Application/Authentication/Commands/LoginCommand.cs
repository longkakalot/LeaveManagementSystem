using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Authentication.Commands
{
    public class LoginCommand : IRequest<string>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? MaChucVu { get; set; }
        public string? MaPhongBan { get; set; }
        public string? TenPhongBan { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? PasswordHash { get; set; }
        public string FullName { get; set; } = default!;
        public string? ReturnUrl { get; set; }
    }
}
