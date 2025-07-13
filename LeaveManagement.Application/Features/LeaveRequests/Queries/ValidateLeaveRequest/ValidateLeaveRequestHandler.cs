using LeaveManagement.Application.Common;
using LeaveManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.ValidateLeaveRequest
{
    public class ValidateLeaveRequestHandler : IRequestHandler<ValidateLeaveRequest, ServiceResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IHolidayRepository _holidayRepo;
        //private readonly ICompensateWorkingDayRepository _compensateRepo;

        public ValidateLeaveRequestHandler(
            //IHolidayRepository holidayRepo,
            //ICompensateWorkingDayRepository compensateRepo,
            IUnitOfWork unitOfWork)
        {
            //_holidayRepo = holidayRepo;
            //_compensateRepo = compensateRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult> Handle(ValidateLeaveRequest request, CancellationToken cancellationToken)
        {
            // Lấy ngày lễ/ngày làm bù trong khoảng ngày xin nghỉ
            var holidays = await _unitOfWork.Holidays.GetHolidaysInRange(request.FromDate, request.ToDate);
            var compensateDays = await _unitOfWork.CompensateWorkingDays.GetCompensateDaysInRange(request.FromDate, request.ToDate);

            // Check có ít nhất 1 ngày làm việc thực sự không

            if (!WorkingDayCalculator.HasAtLeastOneWorkingDay(request.FromDate, request.ToDate, holidays, compensateDays))
            {
                return ServiceResult.Failed("Không thể xin nghỉ phép chỉ vào ngày nghỉ (thứ 7, CN, ngày lễ)!");
            }


            ////Check tất cả ngày đều là ngày làm việc
            //if (!WorkingDayCalculator.IsAllWorkingDays(request.FromDate, request.ToDate, holidays, compensateDays))
            //{
            //    return ServiceResult.Failed("Bạn chỉ được phép xin nghỉ vào các ngày làm việc (không được chọn Thứ 7, Chủ nhật hoặc ngày lễ)!");
            //}

            // Các nghiệp vụ khác: kiểm tra số phép còn lại, kiểm tra ngày liên tục...
            // ...

            return ServiceResult.SuccessResult();
        }
    }

}
