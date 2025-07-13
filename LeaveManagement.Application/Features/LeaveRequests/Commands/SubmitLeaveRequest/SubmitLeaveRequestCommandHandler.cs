using Dapper;
using LeaveManagement.Application.Common;
using LeaveManagement.Application.DTOs;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.SubmitLeaveRequest
{
    //public class SubmitLeaveRequestCommandHandler : IRequestHandler<SubmitLeaveRequestCommand, ServiceResult>
    //{
    //    private readonly ILeaveRequestRepository _leaveRequestRepo;
    //    private readonly IUserRepository _userRepo;
    //    private readonly IApprovalGroupRepository _groupRepo;
    //    private readonly IApprovalStepRepository _stepRepo;
    //    private readonly ILeaveApprovalRequestRepository _approvalRequestRepo;
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly ICurrentUserService _currentUserService;

    //    public SubmitLeaveRequestCommandHandler(
    //        ILeaveRequestRepository leaveRequestRepo,
    //        IUserRepository userRepo,
    //        IApprovalGroupRepository groupRepo,
    //        IApprovalStepRepository stepRepo,
    //        ILeaveApprovalRequestRepository approvalRequestRepo,
    //        IUnitOfWork unitOfWork,
    //        ICurrentUserService currentUserService)
    //    {
    //        _leaveRequestRepo = leaveRequestRepo;
    //        _userRepo = userRepo;
    //        _groupRepo = groupRepo;
    //        _stepRepo = stepRepo;
    //        _approvalRequestRepo = approvalRequestRepo;
    //        _unitOfWork = unitOfWork;
    //        _currentUserService = currentUserService;
    //    }

    //    public async Task<ServiceResult> Handle(SubmitLeaveRequestCommand request, CancellationToken cancellationToken)
    //    {
    //        // 1. Kiểm tra đơn nghỉ phép
    //        var leaveRequest = await _leaveRequestRepo.GetByIdAsync(request.LeaveRequestId);
    //        if (leaveRequest == null || leaveRequest.Status != LeaveStatus.Pending)
    //            return ServiceResult.Failed("Đơn không hợp lệ.");

    //        // 2. Lấy user gửi đơn
    //        //var user = await _userRepo.GetByIdAsync(leaveRequest.UserId);
    //        var user = _currentUserService.UserId.ToString();
    //        if (user == null) return ServiceResult.Failed("Không tìm thấy người gửi đơn.");

    //        // 3. Tính số ngày nghỉ
    //        var days = (leaveRequest.ToDate - leaveRequest.FromDate).Days + 1;

    //        //lấy maChucVu
    //        var maChucVu = _currentUserService.MaChucVu;
    //        var maPhongBan = _currentUserService.MaPhongBan;

    //        //4. lấy workflow duyệt
    //        var steps = await _stepRepo.GetStepsByGroupAsync(maChucVu, days);
    //        if (steps == null || steps.Count == 0)
    //            return ServiceResult.Failed("Chưa cấu hình các bước duyệt.");            

    //        // 6. Xác định người duyệt Step đầu
    //        var firstStep = steps.OrderBy(s => s.StepOrder).First();

    //        var approvalRole = firstStep.ApproverRole;

    //        //Tạo list user người duyệt
    //        var userApprover = new List<User>();            

    //        if(approvalRole == "GD,PGD")
    //        {
    //            userApprover = (await _userRepo.FindApproverAsyncByMaChucVu(approvalRole)).ToList();               

    //        }
    //        else
    //        {
    //            userApprover = (await _userRepo.FindApproverAsync(approvalRole, maPhongBan)).ToList();
    //        }


    //        var userIds = "";

    //        foreach (var userId in userApprover) {
    //            userIds += userId.UserId.ToString() + ",";  
    //        }


    //        //var approvalRoles = string[];
    //        //if (approvalRole.Contains(','))
    //        //{
    //        //    var abc = approvalRole.Split(',');

    //        //}

    //        //var approver = await _userRepo.FindApproverAsync(firstStep.ApproverRole, user.MaPhongBan);
    //        if (userApprover == null)
    //            return ServiceResult.Failed("Không tìm thấy người duyệt.");

    //        // 7. Tạo LeaveApprovalRequest cho Step đầu tiên

    //        var approvalRequest = new LeaveApprovalRequest
    //        {
    //            LeaveRequestId = leaveRequest.Id,
    //            StepOrder = firstStep.StepOrder,
    //            ApproverRole = firstStep.ApproverRole,
    //            ApproverUserIds = userIds,
    //            Status = LeaveApprovalStatus.Pending,
    //            CreatedAt = DateTime.Now
    //        };
    //        await _approvalRequestRepo.AddAsync(approvalRequest);



    //        // 8. Update trạng thái LeaveRequest            
    //        leaveRequest.Status = LeaveStatus.Submitted;
    //        await _leaveRequestRepo.UpdateStatusAsync(leaveRequest);

    //        // 9. Lưu transaction (UnitOfWork)
    //        await _unitOfWork.CommitAsync();

    //        // 10. (Optional) Gửi thông báo cho approver

    //        return ServiceResult.SuccessResult();
    //    }
    //}    

    public class SubmitLeaveRequestCommandHandler : IRequestHandler<SubmitLeaveRequestCommand, ServiceResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        
        public SubmitLeaveRequestCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService
            )
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            
        }

        public async Task<ServiceResult> Handle(SubmitLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Kiểm tra đơn nghỉ phép
                var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(request.LeaveRequestId);
                if (leaveRequest == null || leaveRequest.Status != LeaveStatus.Pending)
                    return ServiceResult.Failed("Đơn không hợp lệ.");

                // 2. Lấy user gửi đơn
                var user = _currentUserService.UserId.ToString();
                if (user == null) return ServiceResult.Failed("Không tìm thấy người gửi đơn.");

                // 3. Tính số ngày nghỉ
                // ... lấy LeaveRequest hoặc lấy data từ request model
                var fromDate = leaveRequest.FromDate;
                var toDate = leaveRequest.ToDate;
                var fromDateType = leaveRequest.FromDateType;
                var toDateType = leaveRequest.ToDateType;

                var holidays = await _unitOfWork.Holidays.GetHolidaysInRange(fromDate, toDate);
                var compensateDays = await _unitOfWork.CompensateWorkingDays.GetCompensateDaysInRange(fromDate, toDate);

                var days = WorkingDayCalculator.GetWorkingDays(
                    fromDate, toDate,
                    fromDateType!, toDateType!,
                    holidays, compensateDays);

                //var days = (leaveRequest.ToDate - leaveRequest.FromDate).Days + 1;

                // 4. lấy maChucVu, maPhongBan
                var maChucVu = _currentUserService.MaChucVu;

                if (maChucVu == "")
                {
                    maChucVu = "NV";
                }

                var maPhongBan = _currentUserService.MaPhongBan;

                // 5. lấy workflow duyệt
                var steps = await _unitOfWork.ApprovalSteps.GetStepsByGroupAsync(maChucVu!, Convert.ToInt32(days));
                if (steps == null || steps.Count == 0)
                    return ServiceResult.Failed("Chưa cấu hình các bước duyệt.");

                // Tổng số step của quy trình này
                var totalStep = steps.Max(s => s.StepOrder);

                // 6. Lấy step đầu tiên
                var firstStep = steps.OrderBy(s => s.StepOrder).First();
                var approvalRole = firstStep.ApproverRole;

                var userApprover = approvalRole == "GD,PGD"
                    ? (await _unitOfWork.Users.FindApproverAsyncByMaChucVu(approvalRole)).ToList()
                    : (await _unitOfWork.Users.FindApproverAsync(approvalRole!, maPhongBan!)).ToList();

                if (userApprover == null || !userApprover.Any())
                    return ServiceResult.Failed("Không tìm thấy người duyệt.");

                var userIds = string.Join(",", userApprover.Select(x => x.UserId));

                // 7. Tạo LeaveApprovalRequest cho StepApprove đầu tiên
                var approvalRequest = new LeaveApprovalRequest
                {
                    LeaveRequestId = leaveRequest.Id,
                    StepOrder = totalStep,
                    StepApprove = 1,
                    ApproverRole = firstStep.ApproverRole,
                    ApproverUserIds = userIds,
                    Status = LeaveApprovalStatus.Pending,
                    CreatedAt = DateTime.Now
                };
                await _unitOfWork.LeaveApprovalRequests.AddAsync(approvalRequest);

                // 8. Update trạng thái LeaveRequest            
                leaveRequest.Status = LeaveStatus.Submitted;
                leaveRequest.TotalLeaveDays = days;
                await _unitOfWork.LeaveRequests.UpdateStatusAsync(leaveRequest);

                // 9. Lưu transaction (UnitOfWork)
                _unitOfWork.Commit();

                // 10. (Optional) Gửi thông báo cho approver

                return ServiceResult.SuccessResult();
            }
            catch (Exception ex)
            {
                var a = ex.Message;
                _unitOfWork.Rollback();
                return ServiceResult.Failed("Có lỗi khi gửi duyệt: " + ex.Message);
            }
        }
    }

}
