using LeaveManagement.Application.Authentication.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserDto user);
    }
}
