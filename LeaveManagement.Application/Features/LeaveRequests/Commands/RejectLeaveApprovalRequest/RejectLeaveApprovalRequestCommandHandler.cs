using LeaveManagement.Application.Common;
using LeaveManagement.Application.Features.LeaveRequests.Commands.ApproveLeaveRequest;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.RejectLeaveApprovalRequest
{
    public class RejectLeaveApprovalRequestCommandHandler : IRequestHandler<RejectLeaveApprovalRequestCommand, ServiceResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public RejectLeaveApprovalRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }        

        public async Task<ServiceResult> Handle(RejectLeaveApprovalRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var approval = await _unitOfWork.LeaveApprovalRequests.GetByIdAsync(request.LeaveApprovalRequestId);
                if (approval == null || approval.Status != LeaveApprovalStatus.Pending)
                    return ServiceResult.Failed("Phiếu duyệt không hợp lệ hoặc đã xử lý.");

                await _unitOfWork.LeaveApprovalRequests.RejectAsync(approval);
                _unitOfWork.Commit();
                return ServiceResult.SuccessResult();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult.Failed("Lỗi khi từ chối phiếu: " + ex.Message);
                
            }
        }
    }
}
