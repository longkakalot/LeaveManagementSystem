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

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.ApproveLeaveRequest
{
    public class ApproveLeaveRequestCommandHandler : IRequestHandler<ApproveLeaveRequestCommand, ServiceResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ApproveLeaveRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ServiceResult> Handle(ApproveLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var approval = await _unitOfWork.LeaveApprovalRequests.GetByIdAsync(request.LeaveApprovalRequestId);
                if (approval == null 
                    || approval.Status == LeaveApprovalStatus.Approved)
                    return ServiceResult.Failed("Phiếu duyệt không hợp lệ hoặc đã xử lý.");

                var approverIds = approval.ApproverUserIds!.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (!approverIds.Contains(request.ApproverId.ToString()))
                    return ServiceResult.Failed("Bạn không có quyền duyệt phiếu này.");

                approval.Status = LeaveApprovalStatus.Approved;
                approval.ApprovedBy = request.ApproverId;
                approval.ApprovedAt = DateTime.Now;
                approval.Comment = request.Comment;
                await _unitOfWork.LeaveApprovalRequests.UpdateAsync(approval);

                


                if (approval.StepApprove < approval.StepOrder)
                {
                    var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(approval.LeaveRequestId);
                    var steps = await _unitOfWork.ApprovalSteps.GetStepsByGroupAsync(
                        leaveRequest.MaChucVu!,
                        (leaveRequest.ToDate - leaveRequest.FromDate).Days + 1);

                    var nextStep = steps.FirstOrDefault(s => s.StepOrder == approval.StepApprove + 1);
                    if (nextStep == null)
                        return ServiceResult.Failed("Không tìm thấy cấu hình bước duyệt tiếp theo.");

                    var userApprover = new List<User>();

                    if(nextStep.ApproverRole == "TPTCCB,PTPTCCB")
                    {
                        userApprover = (await _unitOfWork.Users.FindApproverAsync("TP,PTP", "PTC")).ToList();                        
                    }
                    else if (nextStep.ApproverRole == "GD,PGD")
                    {
                        userApprover = (await _unitOfWork.Users.FindApproverAsyncByMaChucVu(nextStep.ApproverRole)).ToList();
                    }
                    else if (nextStep.ApproverRole == "TPDD,PTPDD")
                    {
                        userApprover = (await _unitOfWork.Users.FindApproverAsync("TP,PTP", "PDD")).ToList();
                    }
                    else
                    {
                        userApprover = (await _unitOfWork.Users.FindApproverAsync(nextStep.ApproverRole!, leaveRequest.MaPhongBan!)).ToList();
                    }


                    //var userApprover = nextStep.ApproverRole == "TPTCCB"
                    //    ? (await _unitOfWork.Users.FindApproverAsyncByMaChucVu(nextStep.ApproverRole)).ToList()
                    //    : (await _unitOfWork.Users.FindApproverAsync(nextStep.ApproverRole, leaveRequest.MaPhongBan)).ToList();

                    //userApprover = nextStep.ApproverRole == "GD,PGD"
                    //    ? (await _unitOfWork.Users.FindApproverAsyncByMaChucVu(nextStep.ApproverRole)).ToList()
                    //    : (await _unitOfWork.Users.FindApproverAsync(nextStep.ApproverRole, leaveRequest.MaPhongBan)).ToList();

                    if (userApprover == null || !userApprover.Any())
                        return ServiceResult.Failed("Không tìm thấy người duyệt cấp tiếp theo.");

                    var userIds = string.Join(",", userApprover.Select(x => x.UserId));

                    var nextApproval = new LeaveApprovalRequest
                    {
                        LeaveRequestId = leaveRequest.Id,
                        StepOrder = approval.StepOrder,
                        StepApprove = approval.StepApprove + 1,
                        ApproverRole = nextStep.ApproverRole,
                        ApproverUserIds = userIds,
                        Status = LeaveApprovalStatus.Pending,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.LeaveApprovalRequests.AddAsync(nextApproval);
                }
                else
                {
                    // 1. Lấy lại đơn nghỉ phép
                    var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(approval.LeaveRequestId);
                    if (leaveRequest == null)
                        return ServiceResult.Failed("Đơn không hợp lệ.");

                    // 3. Tính số ngày nghỉ
                    // ... lấy LeaveRequest hoặc lấy data từ request model
                    var fromDate = leaveRequest.FromDate;
                    var toDate = leaveRequest.ToDate;
                    var fromDateType = leaveRequest.FromDateType;
                    var toDateType = leaveRequest.ToDateType;

                    var holidays = await _unitOfWork.Holidays.GetHolidaysInRange(fromDate, toDate);
                    var compensateDays = await _unitOfWork.CompensateWorkingDays.GetCompensateDaysInRange(fromDate, toDate);

                    var totalLeaveDays = WorkingDayCalculator.GetWorkingDays(
                        fromDate, toDate,
                        fromDateType!, toDateType!,
                        holidays, compensateDays);


                    leaveRequest.TotalLeaveDays = totalLeaveDays;
                    leaveRequest.Status = LeaveStatus.Approved;
                    await _unitOfWork.LeaveRequests.UpdateStatusAsync(leaveRequest);
                }

                _unitOfWork.Commit();
                return ServiceResult.SuccessResult();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult.Failed("Lỗi khi duyệt phiếu: " + ex.Message);
            }
        }
    }

}
