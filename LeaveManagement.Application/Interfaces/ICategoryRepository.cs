using LeaveManagement.Application.DTOs;
using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    public interface ICategoryRepository
    {                
        Task<List<Countries>> GetCountries();
        Task<List<Provinces>> GetProvinces();
        Task<List<Wards>> GetWardsByProvinceId(int provinceId);

    }
}
