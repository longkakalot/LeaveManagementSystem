using LeaveManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.Categories.Queries.GetWards
{
    public class GetWardsByProvinceIdQuery : IRequest<List<Wards>>
    {
        public int ProvinceId { get; set; } 
    }
}
