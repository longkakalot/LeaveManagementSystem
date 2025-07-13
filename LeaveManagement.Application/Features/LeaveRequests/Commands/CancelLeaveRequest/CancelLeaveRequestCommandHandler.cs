using LeaveManagement.Application.Common;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.CancelLeaveRequest
{
    public class CancelLeaveRequestCommandHandler : IRequestHandler<CancelLeaveRequestCommand, ServiceResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CancelLeaveRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ServiceResult> Handle(CancelLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var approval = await _unitOfWork.LeaveApprovalRequests.GetByIdAsync(request.LeaveApprovalRequestId);
                if (approval == null || approval.Status == LeaveApprovalStatus.Pending)
                    return ServiceResult.Failed("Phiếu duyệt không hợp lệ hoặc đã xử lý.");

                var approverIds = approval.ApproverUserIds!.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (!approverIds.Contains(request.ApproverId.ToString()))
                    return ServiceResult.Failed("Bạn không có quyền hủy phiếu này.");

                approval.Status = LeaveApprovalStatus.Canceled;
                approval.ApprovedBy = request.ApproverId;
                approval.ApprovedAt = DateTime.Now;
                approval.Comment = request.Comment;
                await _unitOfWork.LeaveApprovalRequests.UpdateAsync(approval);

                //Lấy bước duyệt + 1 của bước hiện tại và LeaveRequestId để cập nhật lại status của ApproveRequest là Cancel
                var leaveApproveRequest = new LeaveApprovalRequest
                {
                    StepApprove = approval.StepApprove + 1,
                    LeaveRequestId = approval.LeaveRequestId,
                    Status = LeaveApprovalStatus.Canceled,
                };


                var kq = await _unitOfWork.LeaveApprovalRequests.UpdateAsyncByStepApprove(leaveApproveRequest);

                // Cập nhật trạng thái LeaveRequest là Pending
                var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(approval.LeaveRequestId);
                leaveRequest.Status = LeaveStatus.Pending;
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
