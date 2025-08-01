using LeaveManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.Categories.Queries.GetProvinces
{
    public class GetProvincesQuery : IRequest<List<Provinces>>
    {
    }
}
