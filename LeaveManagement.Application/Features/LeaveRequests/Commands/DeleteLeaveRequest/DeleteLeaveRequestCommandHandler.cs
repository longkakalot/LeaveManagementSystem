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

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.DeleteLeaveRequest
{    
    public class DeleteLeaveRequestCommandHandler : IRequestHandler<DeleteLeaveRequestCommand, ServiceResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLeaveRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public async Task<ServiceResult> Handle(DeleteLeaveRequestCommand request, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(request.Id);
        //        if (leaveRequest == null || leaveRequest.Status != (int)LeaveStatus.Pending)
        //            return ServiceResult.Failed("Không thể hủy đơn: " + request.Id);

        //        int userId = leaveRequest.UserId;
        //        int yearNew = leaveRequest.FromDate.Year;
        //        int yearOld = yearNew - 1;
        //        var lastCarryOverDate = new DateTime(yearNew, 3, 31);

        //        // Lấy lại các ngày đã xin nghỉ của đơn này
        //        var holidays = await _unitOfWork.LeaveRequests.GetAllHolidaysAsync(leaveRequest.FromDate, leaveRequest.ToDate);
        //        var compensateDays = await _unitOfWork.LeaveRequests.GetAllCompensateDayAsync(leaveRequest.FromDate, leaveRequest.ToDate);

        //        var leaveDates = WorkingDayCalculator.GetAllWorkingDatesWithType(
        //            leaveRequest.FromDate,
        //            leaveRequest.ToDate,
        //            leaveRequest.FromDateType!,
        //            leaveRequest.ToDateType!,
        //            holidays,
        //            compensateDays
        //        );

        //        // "Trả phép" lại từng ngày (chia theo năm giống CreateHandler)
        //        //double remainOld = 0;
        //        //double remainNew = 0;
        //        var userLeaveBalanceOld = await _unitOfWork.UserLeaveBalances.GetUserLeaveBalance(userId, yearOld);
        //        var userLeaveBalanceNew = await _unitOfWork.UserLeaveBalances.GetUserLeaveBalance(userId, yearNew);

        //        double phepConNamCu = userLeaveBalanceOld?.LeaveDaysRemain ?? 0;
        //        double phepConNamMoi = userLeaveBalanceNew?.LeaveDaysRemain ?? 0;

        //        foreach (var (date, period) in leaveDates)
        //        {
        //            double value = period == "FullDay" ? 1.0 : (period == "Morning" || period == "Afternoon" ? 0.5 : 0);

        //            // Nếu ngày nghỉ <= 31/3 năm mới, và đã từng trừ phép năm cũ, phải trả lại phép năm cũ trước
        //            if (date <= lastCarryOverDate && userLeaveBalanceOld != null)
        //            {
        //                // Trả phép lại vào năm cũ (ưu tiên)
        //                var resultOld = await _unitOfWork.UserLeaveBalances.ReturnUserLeaveBalance(new LeaveManagement.Domain.Entities.UserLeaveBalances
        //                {
        //                    UserId = userId,
        //                    Year = yearOld,
        //                    DaysToReturn = value
        //                });
        //                if (!resultOld)
        //                {
        //                    _unitOfWork.Rollback();
        //                    return ServiceResult.Failed($"Có lỗi khi hoàn phép năm {yearOld}");
        //                }
        //            }
        //            else
        //            {
        //                // Trả phép lại vào năm mới
        //                var resultNew = await _unitOfWork.UserLeaveBalances.ReturnUserLeaveBalance(new LeaveManagement.Domain.Entities.UserLeaveBalances
        //                {
        //                    UserId = userId,
        //                    Year = yearNew,
        //                    DaysToReturn = value
        //                });
        //                if (!resultNew)
        //                {
        //                    _unitOfWork.Rollback();
        //                    return ServiceResult.Failed($"Có lỗi khi hoàn phép năm {yearNew}");
        //                }
        //            }
        //        }

        //        // Xóa đơn nghỉ
        //        var success = await _unitOfWork.LeaveRequests.DeleteAsync(request.Id);

        //        if (success)
        //        {
        //            _unitOfWork.Commit();
        //            return ServiceResult.SuccessResult();
        //        }
        //        else
        //        {
        //            _unitOfWork.Rollback();
        //            return ServiceResult.Failed("Không xóa được LeaveRequest");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _unitOfWork.Rollback();
        //        return ServiceResult.Failed("Có lỗi cập nhật UserLeaveBalances: " + ex.Message);
        //    }
        //}

        public async Task<ServiceResult> Handle(DeleteLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(request.Id);
                if (leaveRequest == null || leaveRequest.Status != (int)LeaveStatus.Pending)
                    return ServiceResult.Failed("Không thể hủy đơn: " + request.Id);

                // Lấy toàn bộ dòng detail đã sinh ra theo LeaveRequestId
                var details = await _unitOfWork.LeaveRequestDetails.GetByLeaveRequestId(leaveRequest.Id);

                // Trả lại phép cho từng dòng detail
                foreach (var detail in details)
                {
                    var result = await _unitOfWork.UserLeaveBalances.ReturnUserLeaveBalance(new LeaveManagement.Domain.Entities.UserLeaveBalances
                    {
                        UserId = leaveRequest.UserId,
                        Year = detail.Year,
                        DaysToReturn = detail.Value
                    });
                    if (!result)
                    {
                        _unitOfWork.Rollback();
                        return ServiceResult.Failed($"Có lỗi khi hoàn phép năm {detail.Year}");
                    }
                }

                // Xóa toàn bộ dòng detail (nếu chưa dùng ON DELETE CASCADE, gọi repo xóa detail trước, nếu đã ON DELETE CASCADE, chỉ cần xóa master)
                // await _unitOfWork.LeaveRequestDetails.DeleteByLeaveRequestId(leaveRequest.Id);

                // Xóa master (LeaveRequest)
                var success = await _unitOfWork.LeaveRequests.DeleteAsync(request.Id);

                if (success)
                {
                    _unitOfWork.Commit();
                    return ServiceResult.SuccessResult();
                }
                else
                {
                    _unitOfWork.Rollback();
                    return ServiceResult.Failed("Không xóa được LeaveRequest");
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult.Failed("Có lỗi cập nhật UserLeaveBalances: " + ex.Message);
            }
        }





        //public async Task<ServiceResult> Handle(DeleteLeaveRequestCommand request, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(request.Id);
        //        if (leaveRequest == null || leaveRequest.Status != (int)LeaveStatus.Pending)
        //            return ServiceResult.Failed("Không thể hủy đơn: " + request.Id);

        //        var soNgayXinNghi = leaveRequest.TotalLeaveDays;                

        //        //Cập nhật số ngày nghỉ vào UserLeaveBalance
        //        var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
        //        {
        //            UserId = request.UserId,
        //            Year = leaveRequest.FromDate.Year,
        //            DaysToReturn = soNgayXinNghi
        //        };               


        //        var success = await _unitOfWork.LeaveRequests.DeleteAsync(request.Id);

        //        var result = await _unitOfWork.UserLeaveBalances.ReturnUserLeaveBalance(userLeaveBalance);

        //        if (success && result)
        //            _unitOfWork.Commit();
        //        else
        //            _unitOfWork.Rollback();

        //        return ServiceResult.SuccessResult();
        //    }
        //    catch(Exception ex)
        //    {

        //        _unitOfWork.Rollback();
        //        return ServiceResult.Failed("Có lỗi cập nhật UserLeaveBalances: " + ex.Message);
        //    }
        //}
    }

}
