using AutoMapper;
using LeaveManagement.Application.DTOs;
using LeaveManagement.Application.Features.LeaveRequests.Queries.GetLeaveRequestById;
using LeaveManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.UserLeaveBalances.Queries
{
    public class UserLeaveBalanceQueryHandler : IRequestHandler<UserLeaveBalanceQuery, UserLeaveBalancesDto>
    {
        private readonly IUnitOfWork _unitOfWork;        
        private readonly IMapper _mapper;
        public UserLeaveBalanceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;            
            _mapper = mapper;
        }
        public async Task<UserLeaveBalancesDto> Handle(UserLeaveBalanceQuery request, CancellationToken cancellationToken)
        {
            var model = await _unitOfWork.UserLeaveBalances.GetUserLeaveBalance(request.UserId, request.Year);

            var result = _mapper.Map<UserLeaveBalancesDto>(model);

            return result;
        }
    }
}
