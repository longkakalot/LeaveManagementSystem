using LeaveManagement.Application.Common;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.EditLeaveRequest
{
    //public class EditLeaveRequestCommandHandler : IRequestHandler<EditLeaveRequestCommand, bool>
    //{
    //    private readonly ILeaveRequestRepository _repository;

    //    public EditLeaveRequestCommandHandler(ILeaveRequestRepository repository)
    //    {
    //        _repository = repository;
    //    }

    //    public async Task<bool> Handle(EditLeaveRequestCommand request, CancellationToken cancellationToken)
    //    {
    //        var leaveRequest = await _repository.GetByIdAsync(request.Id);
    //        if (leaveRequest == null || leaveRequest.Status != (int)LeaveStatus.Pending)
    //            return false;

    //        leaveRequest.UserId = request.UserId;
    //        leaveRequest.FromDate = request.FromDate;
    //        leaveRequest.ToDate = request.ToDate;
    //        leaveRequest.Reason = request.Reason!;

    //        return await _repository.UpdateAsync(leaveRequest);
    //    }
    //}
    public class EditLeaveRequestCommandHandler : IRequestHandler<EditLeaveRequestCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditLeaveRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(EditLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 3. Tính số ngày nghỉ
                // ... lấy LeaveRequest hoặc lấy data từ request model
                var fromDate = request.FromDate;
                var toDate = request.ToDate;
                var fromDateType = request.FromDateType;
                var toDateType = request.ToDateType;

                var holidays = await _unitOfWork.Holidays.GetHolidaysInRange(fromDate, toDate);
                var compensateDays = await _unitOfWork.CompensateWorkingDays.GetCompensateDaysInRange(fromDate, toDate);

                var days = WorkingDayCalculator.GetWorkingDays(
                    fromDate, toDate,
                    fromDateType!, toDateType!,
                    holidays, compensateDays);



                var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(request.Id);
                if (leaveRequest == null || leaveRequest.Status == LeaveStatus.Approved)
                    return false;

                leaveRequest.UserId = request.UserId;
                leaveRequest.FromDate = request.FromDate;
                leaveRequest.ToDate = request.ToDate;
                leaveRequest.FromDateType = request.FromDateType;
                leaveRequest.ToDateType = request.ToDateType;
                leaveRequest.TotalLeaveDays = days;
                leaveRequest.Reason = request.Reason!;
                leaveRequest.VacationPlace = request.VacationPlace!;

                var success = await _unitOfWork.LeaveRequests.UpdateAsync(leaveRequest);

                if (success)
                    _unitOfWork.Commit();
                else
                    _unitOfWork.Rollback();

                return success;
            }
            catch
            {
                _unitOfWork.Rollback();
                return false;
            }
        }
    }

}
