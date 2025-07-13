using LeaveManagement.Application.Authentication.Dtos;
using LeaveManagement.Application.Common;
using LeaveManagement.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace LeaveManagement.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(UserDto user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    //new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username!),
                    new Claim("FullName", user.FullName ?? ""),
                    new Claim("MaChucVu", user.MaChucVu ?? ""), // Mã chức vụ (VD: TK, GD, NV, ...)
                    new Claim("MaPhongBan", user.MaPhongBan ?? ""), // Mã phòng ban
                    new Claim("TenPhongBan", user.TenPhongBan ?? ""), // Tên phòng ban
                    new Claim("NgayPhepCongThem", user.NgayPhepCongThem.ToString() ?? ""), // NgayPhepCongThem
                    new Claim("SoNgayNghiCoBan", user.SoNgayNghiCoBan.ToString() ?? ""), // SoNgayNghiCoBan
                    new Claim("SoNgayPhepNam", user.SoNgayPhepNam.ToString() ?? ""), // SoNgayPhepNam

                    // ... có thể thêm Email, Phone v.v nếu cần
                };

                if (!string.IsNullOrEmpty(user.Role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, user.Role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                throw;
            }
            
        }
    }

}
