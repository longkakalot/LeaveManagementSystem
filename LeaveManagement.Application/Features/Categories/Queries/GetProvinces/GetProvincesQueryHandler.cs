using AutoMapper;
using LeaveManagement.Application.DTOs;
using LeaveManagement.Application.Features.UserLeaveBalances.Queries;
using LeaveManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.Categories.Queries.GetProvinces
{
    public class GetProvincesQueryHandler : IRequestHandler<GetProvincesQuery, List<Provinces>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetProvincesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<Provinces>> Handle(GetProvincesQuery request, CancellationToken cancellationToken)
        {
            var model = await _unitOfWork.Categories.GetProvinces();

            //var result = _mapper.Map<UserLeaveBalancesDto>(model);

            return model.ToList();
        }
    }
}
