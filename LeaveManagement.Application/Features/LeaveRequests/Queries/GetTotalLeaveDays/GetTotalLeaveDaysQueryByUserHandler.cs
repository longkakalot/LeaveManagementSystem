using LeaveManagement.Application.Common;
using LeaveManagement.Application.Features.LeaveRequests.Commands.ApproveLeaveRequest;
using LeaveManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.GetTotalLeaveDays
{
    public class GetTotalLeaveDaysQueryByUserHandler : IRequestHandler<GetTotalLeaveDaysQueryByUser, double>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetTotalLeaveDaysQueryByUserHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<double> Handle(GetTotalLeaveDaysQueryByUser request, CancellationToken cancellationToken)
        {
            
            double soNgayDaNghi = await _unitOfWork.LeaveRequests.GetTotalLeaveDaysAsync(request.UserId, request.Year);
            return soNgayDaNghi;
        }
    }
}
