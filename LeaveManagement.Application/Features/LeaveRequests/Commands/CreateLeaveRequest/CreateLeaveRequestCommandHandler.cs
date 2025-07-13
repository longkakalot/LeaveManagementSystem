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

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.CreateLeaveRequest
{    
    public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, ServiceResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CreateLeaveRequestCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ServiceResult> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //// 1. Lấy số ngày phép được nghỉ trong năm của user
                int soNgayPhepNam = request.SoNgayPhepNam;

                // Lấy tổng số ngày đã nghỉ trong năm (Approved)
                var year = request.FromDate.Year;
                double soNgayDaNghi = await _unitOfWork.LeaveRequests.GetTotalLeaveDaysAsync(request.UserId, year);

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

                if ((soNgayPhepNam - soNgayDaNghi) < days)
                {
                    return ServiceResult.Failed($"Bạn chỉ còn {soNgayPhepNam - soNgayDaNghi} ngày phép, không đủ để xin nghỉ {days} ngày.");
                }

                var entity = new LeaveRequest
                {
                    UserId = request.UserId,
                    FullName = request.FullName,
                    MaChucVu = request.MaChucVu,
                    MaPhongBan = request.MaPhongBan,
                    TenPhongBan = request.TenPhongBan,
                    LeaveTypeId = request.LeaveTypeId,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    FromDateType = request.FromDateType,
                    ToDateType = request.ToDateType,
                    TotalLeaveDays = days,
                    Reason = request.Reason,
                    VacationPlace = request.VacationPlace,
                    Status = 0,
                    RequestedBy = _currentUser.Id.ToString(),
                    RequestedDate = DateTime.Now
                };

                // Gọi qua UnitOfWork, repo đã có transaction thực sự!
                await _unitOfWork.LeaveRequests.CreateAsync(entity);                

                _unitOfWork.Commit();
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
