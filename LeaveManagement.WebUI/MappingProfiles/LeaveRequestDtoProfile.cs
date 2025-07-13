using AutoMapper;
using LeaveManagement.Application.DTOs;
using LeaveManagement.Domain.Entities;
using LeaveManagement.WebUI.ViewModels;

namespace LeaveManagement.WebUI.MappingProfiles
{
    public class LeaveRequestDtoProfile : Profile
    {
        public LeaveRequestDtoProfile()
        {
            CreateMap<Application.DTOs.LeaveRequestDto, ViewModels.LeaveRequestDto>().ReverseMap(); // 2 chiều
            //CreateMap<LeaveApprovalRequest, ViewModels.LeaveApprovalRequestDto>().ReverseMap();
            
            // Map entity sang app DTO
            CreateMap<LeaveApprovalRequest, Application.DTOs.LeaveApprovalRequestDto>().ReverseMap();

            // Map app DTO sang web ViewModel
            CreateMap<Application.DTOs.LeaveApprovalRequestDto, ViewModels.LeaveApprovalRequestDto>().ReverseMap();


        }
    }
}
