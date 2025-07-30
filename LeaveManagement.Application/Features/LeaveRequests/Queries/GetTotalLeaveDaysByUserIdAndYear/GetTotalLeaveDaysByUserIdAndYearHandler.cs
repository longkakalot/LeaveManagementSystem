using AutoMapper;
using LeaveManagement.Application.Authentication.Dtos;
using LeaveManagement.Application.Common;
using LeaveManagement.Application.Features.LeaveRequests.Commands.ApproveLeaveRequest;
using LeaveManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.GetTotalLeaveDaysByUserIdAndYear
{
    public class GetTotalLeaveDaysByUserIdAndYearHandler : IRequestHandler<GetTotalLeaveDaysByUserIdAndYear, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetTotalLeaveDaysByUserIdAndYearHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }
        public async Task<UserDto> Handle(GetTotalLeaveDaysByUserIdAndYear request, CancellationToken cancellationToken)
        {
            
            var user = await _unitOfWork.Users.GetTotalLeaveDaysByUserIdAndYear(request.UserId, request.Year);

            var model = _mapper.Map<UserDto>(user);
            
            return model;
        }
    }
}
