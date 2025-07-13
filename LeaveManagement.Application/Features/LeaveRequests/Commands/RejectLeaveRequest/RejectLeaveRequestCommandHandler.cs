using LeaveManagement.Application.Common;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.RejectLeaveRequest
{
    public class CancelLeaveRequestCommandHandler : IRequestHandler<RejectLeaveRequestCommand, ServiceResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CancelLeaveRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ServiceResult> Handle(RejectLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var approval = await _unitOfWork.LeaveApprovalRequests.GetByIdAsync(request.LeaveApprovalRequestId);
                if (approval == null || approval.Status != LeaveApprovalStatus.Pending)
                    return ServiceResult.Failed("Phiếu duyệt không hợp lệ hoặc đã xử lý.");

                var approverIds = approval.ApproverUserIds!.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (!approverIds.Contains(request.ApproverId.ToString()))
                    return ServiceResult.Failed("Bạn không có quyền từ chối phiếu này.");

                approval.Status = LeaveApprovalStatus.Rejected;
                approval.ApprovedBy = request.ApproverId;
                approval.ApprovedAt = DateTime.Now;
                approval.Comment = request.Comment;
                await _unitOfWork.LeaveApprovalRequests.UpdateAsync(approval);

                // Cập nhật trạng thái LeaveRequest là Rejected
                var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(approval.LeaveRequestId);
                leaveRequest.Status = LeaveStatus.Rejected;
                await _unitOfWork.LeaveRequests.UpdateStatusAsync(leaveRequest);

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
