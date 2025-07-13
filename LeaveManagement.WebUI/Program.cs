using LeaveManagement.Application.Common;
using LeaveManagement.Application.Features.LeaveRequests.Commands.CreateLeaveRequest;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Application.Mapping;
using LeaveManagement.Infrastructure.Configuration;
using LeaveManagement.Infrastructure.Helpers;
using LeaveManagement.Infrastructure.Persistence.Database;
using LeaveManagement.Infrastructure.Repositories;
using LeaveManagement.Infrastructure.Services;
using LeaveManagement.WebUI.MappingProfiles;
using LeaveManagement.WebUI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

//Đăng ký MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateLeaveRequestCommandHandler).Assembly));

// Repo chỉ đọc (cho Query Handler)
builder.Services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
builder.Services.AddScoped<ILeaveApprovalRequestRepository, LeaveApprovalRequestRepository>();


// Các service
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IApproverService, ApproverService>();
//builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
//builder.Services.AddScoped<ICompensateWorkingDayRepository, CompensateWorkingDayRepository>();


//builder.Services.AddScoped<DapperDbContext>();
//builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
//builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
//builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
//builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
//builder.Services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
//builder.Services.AddScoped<ILeaveApprovalRequestRepository, LeaveApprovalRequestRepository>();
//builder.Services.AddScoped<ILeaveApprovalRequestRepository, LeaveApprovalRequestRepository>();
//builder.Services.AddScoped<IApprovalGroupRepository, ApprovalGroupRepository>();
//builder.Services.AddScoped<IApprovalStepRepository, ApprovalStepRepository>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IApproverService, ApproverService>();

// UnitOfWork, connection factory
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddHttpContextAccessor();

// Các cấu hình log ở đây nếu dùng Serilog
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddAutoMapper(
    typeof(UserProfile).Assembly,
    typeof(LeaveRequestProfile).Assembly,
    typeof(LeaveRequestDtoProfile).Assembly
    );

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key!)),
            ValidateIssuerSigningKey = true
        };

        // ✅ Cấu hình đúng: gộp cả hai sự kiện
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.HttpContext.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                context.HandleResponse();

                var returnUrl = context.Request.Path + context.Request.QueryString;
                var redirectUrl = "/Account/Login?returnUrl=" + Uri.EscapeDataString(returnUrl);

                context.Response.StatusCode = StatusCodes.Status302Found;
                context.Response.Headers.Location = redirectUrl;

                return Task.CompletedTask;
            }
        };
    });

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddApplication();

var app = builder.Build();

var loggerFactory = app.Services.GetService<ILoggerFactory>();
loggerFactory.AddFile("Logs/mylog-{Date}.txt");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
