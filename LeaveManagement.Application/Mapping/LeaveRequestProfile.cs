using AutoMapper;
using LeaveManagement.Application.DTOs;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LeaveManagement.Application.Mapping
{
    public class LeaveRequestProfile : Profile
    {
        public LeaveRequestProfile()
        {
            CreateMap<LeaveRequest, LeaveRequestDto>()
                .ForMember(dest => dest.StatusLabel, opt => opt.MapFrom(src => GetStatusLabel(Convert.ToInt32(src.Status))));
        }

        private static string GetStatusLabel(int status)
        {
            return status switch
            {
                (int)LeaveStatus.Pending => "Tạo mới",
                (int)LeaveStatus.Submitted => "Đang chờ duyệt",
                (int)LeaveStatus.Approved => "Đã duyệt",
                (int)LeaveStatus.Rejected => "Từ chối",
                _ => "Không xác định"
            };
        }
    }

}
