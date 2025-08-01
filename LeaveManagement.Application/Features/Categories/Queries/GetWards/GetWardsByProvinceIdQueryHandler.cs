using AutoMapper;
using LeaveManagement.Application.DTOs;
using LeaveManagement.Application.Features.Categories.Queries.GetProvinces;
using LeaveManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.Categories.Queries.GetWards
{
    public class GetWardsByProvinceIdQueryHandler : IRequestHandler<GetWardsByProvinceIdQuery, List<Wards>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetWardsByProvinceIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<Wards>> Handle(GetWardsByProvinceIdQuery request, CancellationToken cancellationToken)
        {
            var model = await _unitOfWork.Categories.GetWardsByProvinceId(request.ProvinceId);
            
            return model.ToList();
        }
    }
}
