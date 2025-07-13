using AutoMapper;
using LeaveManagement.Application.Authentication.Dtos;
using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Map 2 chiều: User <-> UserDto
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
