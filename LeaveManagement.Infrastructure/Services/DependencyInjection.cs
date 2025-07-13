using LeaveManagement.Application.Authentication.Commands;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Infrastructure.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly);
            });

            // ✅ Đăng ký các service cần thiết
            services.AddScoped<IPasswordHasher, PasswordHasher>();         // PasswordHasher là class của bạn
            services.AddScoped<IJwtTokenService, JwtTokenService>();       // JwtTokenService là class của bạn
            services.AddScoped<IDbConnection>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                return new SqlConnection(connectionString);                // dùng SqlClient
            });

            return services;
        }
    }

}
