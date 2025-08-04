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
    public class EditLeaveRequestCommandHandler : IRequestHandler<EditLeaveRequestCommand, ServiceResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public EditLeaveRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ServiceResult> Handle(EditLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                #region Delete đơn
                double soNgayNghiDonEdit = 0.0;

                var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(request.Id);
                if (leaveRequest == null ||
                    (leaveRequest.Status != LeaveStatus.Pending && leaveRequest.Status != LeaveStatus.Submitted))
                {
                    return ServiceResult.Failed("Chỉ được chỉnh sửa đơn khi ở trạng thái Chờ duyệt hoặc Đã gửi duyệt!");
                }

                //// Lấy toàn bộ dòng detail đã sinh ra theo LeaveRequestId
                //var leaveDetails = await _unitOfWork.LeaveRequestDetails.GetByLeaveRequestId(leaveRequest.Id);

                //// Trả lại phép cho từng dòng detail
                //foreach (var detail in leaveDetails)
                //{
                //    soNgayNghiDonEdit += detail.Value;
                //    var result = await _unitOfWork.UserLeaveBalances.ReturnUserLeaveBalance(new LeaveManagement.Domain.Entities.UserLeaveBalances
                //    {
                //        UserId = leaveRequest.UserId,
                //        Year = detail.Year,
                //        DaysToReturn = detail.Value
                //    });
                //    if (!result)
                //    {
                //        _unitOfWork.Rollback();
                //        return ServiceResult.Failed($"Có lỗi khi hoàn phép năm {detail.Year}");
                //    }
                //}

                // Xóa toàn bộ dòng detail (nếu chưa dùng ON DELETE CASCADE, gọi repo xóa detail trước, nếu đã ON DELETE CASCADE, chỉ cần xóa master)                
                // Xóa master (LeaveRequest)
                var sucessDelete = await _unitOfWork.LeaveRequests.DeleteAsync(request.Id);

                #endregion Delete Đơn

                #region Kiểm tra ngày trùng
                int userId = request.UserId;
                int yearNew = 0;         // Năm của kỳ nghỉ (thường là năm mới)
                int yearOld = 0;                   // Năm trước kỳ nghỉ (năm cũ)


                if (request.FromDate.Year == request.ToDate.Year)
                {
                    yearOld = request.FromDate.Year;
                    yearNew = yearOld + 1;
                }
                else
                {
                    yearNew = request.ToDate.Year;
                    yearOld = request.FromDate.Year;
                }

                var lastCarryOverDate = new DateTime(yearNew, 3, 31);
                var lastYearDate = new DateTime(yearOld, 12, 31);

                // Lấy số phép còn lại năm cũ và năm mới
                double soNgayDaNghiNamCu = await _unitOfWork.LeaveRequests.GetTotalLeaveDaysAsync(request.UserId, yearOld);

                double phepConNamCu = _currentUserService.SoNgayPhepNam - soNgayDaNghiNamCu;


                double soNgayDaNghiNamMoi = await _unitOfWork.LeaveRequests.GetTotalLeaveDaysAsync(request.UserId, yearNew);

                double phepConNamMoi = _currentUserService.SoNgayPhepNam - soNgayDaNghiNamMoi;

                // Lấy ngày nghỉ hợp lệ theo đơn
                var holidays = await _unitOfWork.LeaveRequests.GetAllHolidaysAsync(request.FromDate, request.ToDate);
                var compensateDays = await _unitOfWork.LeaveRequests.GetAllCompensateDayAsync(request.FromDate, request.ToDate);

                double totalLeaveDays = WorkingDayCalculator.GetWorkingDays(
                    request.FromDate, request.ToDate,
                    request.FromDateType!, request.ToDateType!,
                    holidays, compensateDays);

                // Kiểm tra ngày xin nghỉ có trùng các đơn trước chưa?
                var requests = await _unitOfWork.LeaveRequests.GetAllByUserId(userId);
                var allUserDates = WorkingDayCalculator.GetAllUserUsedLeaveDates(requests, holidays, compensateDays);

                var newLeaveDates = WorkingDayCalculator.GetAllWorkingDatesWithType(
                    request.FromDate,
                    request.ToDate,
                    request.FromDateType!,
                    request.ToDateType!,
                    holidays,
                    compensateDays
                );


                foreach (var (date, period) in newLeaveDates) // newLeaveDates: List<(DateTime, string)>
                {
                    if (HasConflict(allUserDates, (date, period)))
                    {
                        return ServiceResult.Failed($"Ngày {date:dd/MM/yyyy} ({(period == "FullDay" ? "Nguyên ngày" : period == "Morning" ? "Sáng" : "Chiều")}) đã có đơn nghỉ phép, vui lòng chọn ngày khác.");
                    }
                }

                #endregion Kiểm tra ngày trùng

                #region Tạo đơn mới                





                //var trungNgay = newLeaveDates.Where(x => allUserDates.Contains(x)).ToList();
                //if (trungNgay.Any())
                //{
                //    return ServiceResult.Failed("Ngày " + string.Join(", ", trungNgay.Select(x => x.Date.ToString("dd/MM/yyyy"))) + " đã có đơn nghỉ phép, vui lòng chọn ngày khác.");
                //}

                // Kiểm tra tổng số phép đủ không
                if ((phepConNamCu + phepConNamMoi) < totalLeaveDays)
                {
                    return ServiceResult.Failed($"Bạn chỉ còn tổng {phepConNamCu + phepConNamMoi} ngày phép, không đủ để xin nghỉ {totalLeaveDays} ngày.");
                }

                // -------------- NGHIỆP VỤ CHIA PHÉP THEO NGÀY - Ưu tiên phép cũ đến hết 31/3 năm mới ----------------
                var details = new List<LeaveRequestDetail>();

                foreach (var (date, period) in newLeaveDates)
                {
                    double value = period == "FullDay" ? 1.0 : (period == "Morning" || period == "Afternoon" ? 0.5 : 0);

                    //Nếu ngày nghỉ bắc cầu, trong đó ngày FromDate <=31/12 thì sẽ kiểm tra còn đủ ngày nghỉ không?
                    if (date <= lastYearDate && (phepConNamCu + soNgayNghiDonEdit) < value)
                    {
                        return ServiceResult.Failed($"Phép năm {yearOld} của bạn còn {phepConNamCu + soNgayNghiDonEdit} ngày, không đủ để xin nghỉ {totalLeaveDays} ngày.");
                    }

                    // Nếu ngày nghỉ <= 31/3 năm mới, còn phép năm cũ thì ưu tiên trừ phép năm cũ
                    if (date <= lastCarryOverDate && phepConNamCu > 0)
                    {
                        if (phepConNamCu >= value)
                        {
                            // Trừ hết vào phép năm cũ
                            //var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
                            //{
                            //    UserId = userId,
                            //    Year = yearOld,
                            //    DaysToDeduct = value
                            //};
                            //var kq1 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
                            //if (!kq1)
                            //{
                            //    _unitOfWork.Rollback();
                            //    return ServiceResult.Failed("Có lỗi khi cập nhật phép năm " + yearOld);
                            //}

                            details.Add(new LeaveRequestDetail
                            {
                                Date = date,
                                Period = period,
                                Year = yearOld,
                                Value = value,
                                // LeaveRequestId = sẽ set sau khi có Id
                            });

                            phepConNamCu -= value;
                        }
                        else
                        {
                            // Nếu phép năm cũ không đủ cho value, trừ nốt phần còn lại năm cũ, phần còn lại sang năm mới
                            double remainOld = phepConNamCu;
                            double remainNew = value - phepConNamCu;

                            if (remainOld > 0)
                            {
                                //var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
                                //{
                                //    UserId = userId,
                                //    Year = yearOld,
                                //    DaysToDeduct = remainOld
                                //};
                                //var kq1 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
                                //if (!kq1)
                                //{
                                //    _unitOfWork.Rollback();
                                //    return ServiceResult.Failed("Có lỗi khi cập nhật phép năm " + yearOld);
                                //}

                                details.Add(new LeaveRequestDetail
                                {
                                    Date = date,
                                    Period = period,
                                    Year = yearOld,
                                    Value = phepConNamCu,
                                    // LeaveRequestId = sẽ set sau khi có Id
                                });

                                phepConNamCu = 0;
                            }

                            if (remainNew > 0)
                            {
                                //var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
                                //{
                                //    UserId = userId,
                                //    Year = yearNew,
                                //    DaysToDeduct = remainNew
                                //};
                                //var kq2 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
                                //if (!kq2)
                                //{
                                //    _unitOfWork.Rollback();
                                //    return ServiceResult.Failed("Có lỗi khi cập nhật phép năm " + yearNew);
                                //}

                                details.Add(new LeaveRequestDetail
                                {
                                    Date = date,
                                    Period = period,
                                    Year = yearNew,
                                    Value = remainNew,
                                    // LeaveRequestId = sẽ set sau khi có Id
                                });

                                phepConNamMoi -= remainNew;
                            }
                        }
                    }
                    else
                    {
                        // Sau 31/3 năm mới hoặc phép năm cũ đã hết, chỉ trừ phép năm mới
                        //var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
                        //{
                        //    UserId = userId,
                        //    Year = yearNew,
                        //    DaysToDeduct = value
                        //};
                        //var kq2 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
                        //if (!kq2)
                        //{
                        //    _unitOfWork.Rollback();
                        //    return ServiceResult.Failed("Có lỗi khi cập nhật phép năm " + yearNew);
                        //}

                        details.Add(new LeaveRequestDetail
                        {
                            Date = date,
                            Period = period,
                            Year = yearNew,
                            Value = value,
                            // LeaveRequestId = sẽ set sau khi có Id
                        });

                        phepConNamMoi -= value;
                    }
                }

                //var userId = Convert.ToInt32(_currentUserService.UserId);
                var maChucVu = string.IsNullOrEmpty(_currentUserService.MaChucVu) ? "NV" : _currentUserService.MaChucVu;
                var maPhongBan = _currentUserService.MaPhongBan;
                var tenPhongBan = _currentUserService.TenPhongBan;
                var fullname = _currentUserService.FullName;

                // Tạo đơn nghỉ phép
                var entity = new LeaveRequest
                {
                    UserId = request.UserId,
                    FullName = fullname,
                    MaChucVu = maChucVu,
                    MaPhongBan = maPhongBan,
                    TenPhongBan = tenPhongBan,
                    LeaveTypeId = 1,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    FromDateType = request.FromDateType,
                    ToDateType = request.ToDateType,
                    TotalLeaveDays = totalLeaveDays,
                    Reason = request.Reason,
                    VacationPlace = request.VacationPlace,
                    Status = 0,
                    RequestedBy = _currentUserService.Id.ToString(),
                    RequestedDate = DateTime.Now,
                    CountryName = request.CountryName,
                    ProvinceName = request.ProvinceName,
                    WardName = request.WardName
                    // Nếu cần, có thể ghi chú lại số ngày trừ vào từng năm
                };

                var kqCreate = await _unitOfWork.LeaveRequests.CreateAsync(entity);
                // Sau khi có Id, insert detail
                if (kqCreate >= 1 && sucessDelete)
                {
                    var leaveRequestId = kqCreate; // hoặc lấy lại Id sau insert (tùy repo)

                    // Gán LeaveRequestId cho từng detail trước khi insert
                    foreach (var d in details)
                        d.LeaveRequestId = leaveRequestId;

                    await _unitOfWork.LeaveRequestDetails.AddRangeAsync(details);

                    _unitOfWork.Commit();
                    return ServiceResult.SuccessResult();
                }
                else
                {
                    _unitOfWork.Rollback();
                    return ServiceResult.Failed("Lỗi khi lưu đơn nghỉ phép!");
                }
                #endregion Tạo đơn mới



            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult.Failed("Có lỗi khi cập nhật đơn: " + ex.Message);
            }
        }

        bool HasConflict(IEnumerable<(DateTime Date, string Period)> usedDates, (DateTime Date, string Period) newDate)
        {
            var sameDateItems = usedDates.Where(x => x.Date.Date == newDate.Date.Date).ToList();

            if (!sameDateItems.Any()) return false;

            // Nếu đơn mới là FullDay, chỉ cần đã nghỉ Sáng, Chiều hoặc FullDay là cấm
            if (newDate.Period == "FullDay")
            {
                if (sameDateItems.Any(x => x.Period == "FullDay" || x.Period == "Morning" || x.Period == "Afternoon"))
                    return true;
            }
            // Nếu đơn mới là Morning, chỉ cần đã nghỉ FullDay hoặc Morning là cấm
            else if (newDate.Period == "Morning")
            {
                if (sameDateItems.Any(x => x.Period == "FullDay" || x.Period == "Morning"))
                    return true;
            }
            // Nếu đơn mới là Afternoon, chỉ cần đã nghỉ FullDay hoặc Afternoon là cấm
            else if (newDate.Period == "Afternoon")
            {
                if (sameDateItems.Any(x => x.Period == "FullDay" || x.Period == "Afternoon"))
                    return true;
            }

            return false;
        }




        //public async Task<bool> Handle(EditLeaveRequestCommand request, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        #region Delete đơn


        //        #endregion Delete Đơn



        //        // 3. Tính số ngày nghỉ
        //        // ... lấy LeaveRequest hoặc lấy data từ request model
        //        var fromDate = request.FromDate;
        //        var toDate = request.ToDate;
        //        var fromDateType = request.FromDateType;
        //        var toDateType = request.ToDateType;

        //        var holidays = await _unitOfWork.LeaveRequests.GetAllHolidaysAsync(fromDate, toDate);
        //        var compensateDays = await _unitOfWork.LeaveRequests.GetAllCompensateDayAsync(fromDate, toDate);

        //        var days = WorkingDayCalculator.GetWorkingDays(
        //            fromDate, toDate,
        //            fromDateType!, toDateType!,
        //            holidays, compensateDays);



        //        var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(request.Id);
        //        if (leaveRequest == null || leaveRequest.Status == LeaveStatus.Approved)
        //            return false;

        //        leaveRequest.UserId = request.UserId;
        //        leaveRequest.FromDate = request.FromDate;
        //        leaveRequest.ToDate = request.ToDate;
        //        leaveRequest.FromDateType = request.FromDateType;
        //        leaveRequest.ToDateType = request.ToDateType;
        //        leaveRequest.TotalLeaveDays = days;
        //        leaveRequest.Reason = request.Reason!;
        //        leaveRequest.VacationPlace = request.VacationPlace!;

        //        var success = await _unitOfWork.LeaveRequests.UpdateAsync(leaveRequest);

        //        if (success)
        //            _unitOfWork.Commit();
        //        else
        //            _unitOfWork.Rollback();

        //        return success;
        //    }
        //    catch
        //    {
        //        _unitOfWork.Rollback();
        //        return false;
        //    }
        //}
    }

}
