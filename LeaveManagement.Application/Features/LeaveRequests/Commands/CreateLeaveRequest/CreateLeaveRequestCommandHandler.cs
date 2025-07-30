using AutoMapper;
using LeaveManagement.Application.Authentication.Dtos;
using LeaveManagement.Application.Common;
using LeaveManagement.Application.DTOs;
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
    //public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, ServiceResult>
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly ICurrentUserService _currentUser;

    //    public CreateLeaveRequestCommandHandler(
    //        IUnitOfWork unitOfWork,
    //        ICurrentUserService currentUser)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _currentUser = currentUser;
    //    }

    //    public async Task<ServiceResult> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            //// 1. Lấy số ngày phép được nghỉ trong năm của user
    //            int soNgayPhepNam = request.SoNgayPhepNam;

    //            // Lấy tổng số ngày đã nghỉ trong năm (Approved)
    //            var year = request.FromDate.Year;
    //            double soNgayDaNghi = await _unitOfWork.LeaveRequests.GetTotalLeaveDaysAsync(request.UserId, year);

    //            // 3. Tính số ngày nghỉ
    //            // ... lấy LeaveRequest hoặc lấy data từ request model
    //            var fromDate = request.FromDate;
    //            var toDate = request.ToDate;
    //            var fromDateType = request.FromDateType;
    //            var toDateType = request.ToDateType;

    //            var holidays = await _unitOfWork.LeaveRequests.GetAllHolidaysAsync(fromDate, toDate);
    //            var compensateDays = await _unitOfWork.LeaveRequests.GetAllCompensateDayAsync(fromDate, toDate);

    //            var days = WorkingDayCalculator.GetWorkingDays(
    //                fromDate, toDate,
    //                fromDateType!, toDateType!,
    //                holidays, compensateDays);

    //            if ((soNgayPhepNam - soNgayDaNghi) < days)
    //            {
    //                return ServiceResult.Failed($"Bạn chỉ còn {soNgayPhepNam - soNgayDaNghi} ngày phép, không đủ để xin nghỉ {days} ngày.");
    //            }

    //            var entity = new LeaveRequest
    //            {
    //                UserId = request.UserId,
    //                FullName = request.FullName,
    //                MaChucVu = request.MaChucVu,
    //                MaPhongBan = request.MaPhongBan,
    //                TenPhongBan = request.TenPhongBan,
    //                LeaveTypeId = request.LeaveTypeId,
    //                FromDate = request.FromDate,
    //                ToDate = request.ToDate,
    //                FromDateType = request.FromDateType,
    //                ToDateType = request.ToDateType,
    //                TotalLeaveDays = days,
    //                Reason = request.Reason,
    //                VacationPlace = request.VacationPlace,
    //                Status = 0,
    //                RequestedBy = _currentUser.Id.ToString(),
    //                RequestedDate = DateTime.Now
    //            };

    //            // Gọi qua UnitOfWork, repo đã có transaction thực sự!
    //            await _unitOfWork.LeaveRequests.CreateAsync(entity);

    //            _unitOfWork.Commit();
    //            return ServiceResult.SuccessResult();
    //        }
    //        catch (Exception ex)
    //        {
    //            var a = ex.Message;
    //            _unitOfWork.Rollback();
    //            return ServiceResult.Failed("Có lỗi khi gửi duyệt: " + ex.Message);
    //        }
    //    }
    //}

    public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, ServiceResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public CreateLeaveRequestCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<ServiceResult> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                

                int userId = request.UserId;
                int yearNew = 0;         // Năm của kỳ nghỉ (thường là năm mới)
                int yearOld = 0;                   // Năm trước kỳ nghỉ (năm cũ)


                if(request.FromDate.Year == request.ToDate.Year)
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
                var userLeaveBalanceNamCu = await _unitOfWork.UserLeaveBalances.GetUserLeaveBalance(userId, yearOld);
                double phepConNamCu = userLeaveBalanceNamCu?.LeaveDaysRemain ?? 0;

                var userLeaveBalanceNamMoi = await _unitOfWork.UserLeaveBalances.GetUserLeaveBalance(userId, yearNew);
                double phepConNamMoi = userLeaveBalanceNamMoi?.LeaveDaysRemain ?? 0;

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

                foreach (var item in allUserDates)
                {
                    if(request.FromDate == item.Date && item.Period == "FullDay")
                    {
                        return ServiceResult.Failed("Ngày " + string.Join(", ", request.FromDate.Date.ToString("dd/MM/yyyy") + " đã có đơn nghỉ phép, vui lòng chọn ngày khác."));

                    }
                    if (request.ToDate == item.Date && item.Period == "FullDay")
                    {
                        return ServiceResult.Failed("Ngày " + string.Join(", ", request.ToDate.Date.ToString("dd/MM/yyyy") + " đã có đơn nghỉ phép, vui lòng chọn ngày khác."));

                    }
                }

                var trungNgay = newLeaveDates.Where(x => allUserDates.Contains(x)).ToList();
                if (trungNgay.Any())
                {
                    return ServiceResult.Failed("Ngày " + string.Join(", ", trungNgay.Select(x => x.Date.ToString("dd/MM/yyyy"))) + " đã có đơn nghỉ phép, vui lòng chọn ngày khác.");
                }

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
                    if(date <= lastYearDate && phepConNamCu < value)
                    {
                        return ServiceResult.Failed($"Phép năm {yearOld} của bạn chỉ còn {phepConNamCu}, không đủ để xin nghỉ {value} ngày.");
                    }

                    // Nếu ngày nghỉ <= 31/3 năm mới, còn phép năm cũ thì ưu tiên trừ phép năm cũ
                    if (date <= lastCarryOverDate && phepConNamCu > 0)
                    {
                        if (phepConNamCu >= value)
                        {
                            // Trừ hết vào phép năm cũ
                            var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
                            {
                                UserId = userId,
                                Year = yearOld,
                                DaysToDeduct = value
                            };
                            var kq1 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
                            if (!kq1)
                            {
                                _unitOfWork.Rollback();
                                return ServiceResult.Failed("Có lỗi khi cập nhật phép năm " + yearOld);
                            }

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
                                var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
                                {
                                    UserId = userId,
                                    Year = yearOld,
                                    DaysToDeduct = remainOld
                                };
                                var kq1 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
                                if (!kq1)
                                {
                                    _unitOfWork.Rollback();
                                    return ServiceResult.Failed("Có lỗi khi cập nhật phép năm " + yearOld);
                                }

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
                                var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
                                {
                                    UserId = userId,
                                    Year = yearNew,
                                    DaysToDeduct = remainNew
                                };
                                var kq2 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
                                if (!kq2)
                                {
                                    _unitOfWork.Rollback();
                                    return ServiceResult.Failed("Có lỗi khi cập nhật phép năm " + yearNew);
                                }

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
                        var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
                        {
                            UserId = userId,
                            Year = yearNew,
                            DaysToDeduct = value
                        };
                        var kq2 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
                        if (!kq2)
                        {
                            _unitOfWork.Rollback();
                            return ServiceResult.Failed("Có lỗi khi cập nhật phép năm " + yearNew);
                        }

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

                // Tạo đơn nghỉ phép
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
                    TotalLeaveDays = totalLeaveDays,
                    Reason = request.Reason,
                    VacationPlace = request.VacationPlace,
                    Status = 0,
                    RequestedBy = _currentUser.Id.ToString(),
                    RequestedDate = DateTime.Now
                    // Nếu cần, có thể ghi chú lại số ngày trừ vào từng năm
                };

                var kqCreate = await _unitOfWork.LeaveRequests.CreateAsync(entity);
                // Sau khi có Id, insert detail
                if (kqCreate >= 1)
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
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult.Failed("Có lỗi khi gửi duyệt: " + ex.Message);
            }
        }



        //public async Task<ServiceResult> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
        //{
        //    try
        //    {                
        //        int userId = request.UserId;
        //        int yearFrom = request.FromDate.Year;
        //        int yearTo = request.ToDate.Year;

        //        int yearCurrent = DateTime.Now.Year;

        //        var lastDateOfOldYear = new DateTime(yearFrom, 12, 31);
        //        var lastCarryOverDate = new DateTime(yearFrom + 1, 3, 31);

        //        // 1. Lấy số ngày phép được nghỉ trong năm hiện tại của user
        //        int soNgayPhepNamCu = request.SoNgayPhepNam;

        //        // Lấy tổng số ngày đã nghỉ trong năm hiện tại (Approved)
        //        //double soNgayDaNghiNamCu = await _unitOfWork.LeaveRequests.GetTotalLeaveDaysAsync(request.UserId, request.FromDate.Year);

        //        var a = await _unitOfWork.UserLeaveBalances.GetUserLeaveBalance(userId, yearFrom);
        //        double soNgayDaNghiNamCu = a.LeaveDaysTaken;                

        //        //TH1: nếu thời gian xin nghỉ trong năm

        //        if (request.FromDate <= lastDateOfOldYear && request.ToDate <= lastDateOfOldYear && yearFrom == yearCurrent)
        //        {
        //            if (soNgayPhepNamCu <= 0)
        //            {
        //                return ServiceResult.Failed("Bạn đã hết phép năm " + yearFrom + ". Vui lòng chọn kỳ nghỉ bắt đầu từ " + (yearFrom + 1) + ".");
        //            }

        //            //lấy danh sách ngày nghỉ lễ
        //            var holidaysNamCu = await _unitOfWork.LeaveRequests.GetAllHolidaysAsync(request.FromDate, request.ToDate);

        //            //Lấy danh sách ngày làm bù
        //            var compensateDaysNamCu = await _unitOfWork.LeaveRequests.GetAllCompensateDayAsync(request.FromDate, request.ToDate);

        //            //Lấy số ngày làm việc hợp lệ theo đơn nghỉ phép
        //            double totalLeaveDaysNamCu = WorkingDayCalculator.GetWorkingDays(
        //                request.FromDate, request.ToDate,
        //                request.FromDateType!, request.ToDateType!,
        //                holidaysNamCu, compensateDaysNamCu);

        //            /////KIỂM TRA NGÀY PHÉP TRÙNG VỚI NGÀY ĐÃ XIN NGHỈ
        //            //Lấy toàn bộ ngày phép đã xin nghỉ
        //            var requests = await _unitOfWork.LeaveRequests.GetAllByUserId(userId);
        //            var allUserDates = WorkingDayCalculator.GetAllUserUsedLeaveDates(requests, holidaysNamCu, compensateDaysNamCu);                                       

        //            //Lấy danh sách các ngày sẽ xin nghỉ                    
        //            var newLeaveDates = WorkingDayCalculator.GetAllWorkingDatesWithType(
        //                request.FromDate,
        //                request.ToDate,
        //                request.FromDateType!,
        //                request.ToDateType!,
        //                holidaysNamCu,
        //                compensateDaysNamCu
        //            );

        //            //Kiểm tra trùng
        //            var trungNgay = newLeaveDates.Where(x => allUserDates.Contains(x)).ToList();                    
        //            if (trungNgay.Any())
        //            {
        //                //return ServiceResult.Failed("Ngày bị trùng ");
        //                return ServiceResult.Failed("Ngày " + string.Join(", ", trungNgay.Select(x => x.Date.ToString("dd/MM/yyyy"))) + " đã có đơn nghỉ phép, vui lòng chọn ngày khác.");
        //            }

        //            if ((soNgayPhepNamCu - soNgayDaNghiNamCu) < totalLeaveDaysNamCu)
        //            {
        //                return ServiceResult.Failed($"Bạn chỉ còn {soNgayPhepNamCu - soNgayDaNghiNamCu} ngày phép, không đủ để xin nghỉ {totalLeaveDaysNamCu} ngày.");
        //            }

        //            //Tạo đơn nghỉ

        //            var entity1 = new LeaveRequest
        //            {
        //                UserId = request.UserId,
        //                FullName = request.FullName,
        //                MaChucVu = request.MaChucVu,
        //                MaPhongBan = request.MaPhongBan,
        //                TenPhongBan = request.TenPhongBan,
        //                LeaveTypeId = request.LeaveTypeId,
        //                FromDate = request.FromDate,
        //                ToDate = request.ToDate,
        //                FromDateType = request.FromDateType,
        //                ToDateType = request.ToDateType,
        //                TotalLeaveDays = totalLeaveDaysNamCu,
        //                Reason = request.Reason,
        //                VacationPlace = request.VacationPlace,
        //                Status = 0,
        //                RequestedBy = _currentUser.Id.ToString(),
        //                RequestedDate = DateTime.Now
        //            };

        //            var kq1 = await _unitOfWork.LeaveRequests.CreateAsync(entity1);

        //            //Cập nhật số ngày nghỉ vào UserLeaveBalance
        //            var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
        //            {
        //                UserId = userId,
        //                Year = yearFrom,
        //                DaysToDeduct = totalLeaveDaysNamCu
        //            };
        //            var kq2 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);

        //            if(kq1 >=1 && kq2)
        //            {
        //                _unitOfWork.Commit();

        //            }
        //            else
        //            {
        //                _unitOfWork.Rollback();
        //                return ServiceResult.Failed("Lỗi tạo đơn: LeaveRequest: " + kq1 + "DeductUserLeaveBalance: " + kq2);
        //            }

        //        }

        //        //TH2: xin nghỉ năm này và năm sau
        //        else if (request.FromDate <= lastDateOfOldYear && request.ToDate > lastDateOfOldYear)
        //        {
        //            //Kiểm tra phép năm cũ, nếu không còn thì warning
        //            if (soNgayDaNghiNamCu <= 0)
        //            {
        //                return ServiceResult.Failed("Bạn đã hết phép năm " + yearFrom + ". Vui lòng chọn kỳ nghỉ bắt đầu từ " + (yearFrom + 1) + ".");
        //            }                    

        //            // Lấy ngày lễ/ngày bù
        //            var holidays1 = await _unitOfWork.LeaveRequests.GetAllHolidaysAsync(request.FromDate, request.ToDate);
        //            var compensateDays1 = await _unitOfWork.LeaveRequests.GetAllCompensateDayAsync(request.FromDate, request.ToDate);

        //            double totalLeaveDays = WorkingDayCalculator.GetWorkingDays(
        //                request.FromDate, request.ToDate,
        //                request.FromDateType!, request.ToDateType!,
        //                holidays1, compensateDays1);

        //            // Tính tất cả ngày nghỉ
        //            var days = WorkingDayCalculator.GetAllWorkingDatesWithType(
        //                request.FromDate, request.ToDate, request.FromDateType!, request.ToDateType!, holidays1, compensateDays1
        //            );

        //            // Lấy phép còn lại từng năm
        //            var userLeaveBalanceNamCu = await _unitOfWork.UserLeaveBalances.GetUserLeaveBalance(userId, yearFrom);
        //            var phepConNamCu = userLeaveBalanceNamCu.LeaveDaysRemain;
        //            var phepDaSuDungNamCu = userLeaveBalanceNamCu.LeaveDaysTaken;

        //            var userLeaveBalanceNamMoi = await _unitOfWork.UserLeaveBalances.GetUserLeaveBalance(userId, yearTo);
        //            var phepConNamMoi = userLeaveBalanceNamMoi.LeaveDaysRemain;
        //            var phepDaSuDungNamMoi = userLeaveBalanceNamMoi.LeaveDaysTaken;

        //            //Lấy ngày lễ/ngày bù năm cũ
        //            var holidaysNamCu = await _unitOfWork.LeaveRequests.GetAllHolidaysAsync(request.FromDate, lastDateOfOldYear);
        //            var compensateDaysNamCu = await _unitOfWork.LeaveRequests.GetAllCompensateDayAsync(request.FromDate, lastDateOfOldYear);

        //            double totalLeaveDaysNamCu = WorkingDayCalculator.GetWorkingDays(
        //                request.FromDate, lastDateOfOldYear,
        //                request.FromDateType!, request.ToDateType!,
        //                holidaysNamCu, compensateDaysNamCu);

        //            if ((phepConNamCu) < totalLeaveDaysNamCu)
        //            {
        //                return ServiceResult.Failed("Bạn đã hết phép năm " + yearFrom + ". Vui lòng chọn lại.");

        //            }

        //            if((phepConNamCu + phepConNamMoi) < totalLeaveDays)
        //            {
        //                return ServiceResult.Failed("Bạn chỉ có tổng phép là: " + (phepConNamCu + phepConNamMoi) + ". Không đủ để nghỉ " + totalLeaveDays + " ngày!");
        //            }

        //            //double phepConNamCu = ...; // phép năm 2025 còn lại
        //            //double phepConNamMoi = ...; // phép năm 2026 còn lại

        //            foreach (var (date, period) in days)
        //            {
        //                double value = period == "FullDay" ? 1.0 : (period == "Morning" || period == "Afternoon" ? 0.5 : 0);

        //                // 1. Được dùng phép năm 2025 cho đến hết 31/3/2026, nếu còn phép năm 2025
        //                if (date <= new DateTime(yearFrom + 1, 3, 31) && phepConNamCu > 0)
        //                {
        //                    if (phepConNamCu >= value)
        //                    {
        //                        // Trừ hoàn toàn vào năm cũ
        //                        var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
        //                        {
        //                            UserId = userId,
        //                            Year = yearFrom,
        //                            DaysToDeduct = value
        //                        };

        //                        var kq1 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
        //                        if (!kq1)
        //                        {
        //                            _unitOfWork.Rollback();
        //                            return ServiceResult.Failed("Có lỗi khi cập nhật phép năm cũ");
        //                        }

        //                        phepConNamCu -= value;
        //                    }
        //                    else
        //                    {
        //                        // Chỉ còn lại một phần phép năm cũ, phần còn lại chuyển sang năm mới
        //                        double remainOld = phepConNamCu;
        //                        double remainNew = value - phepConNamCu;

        //                        // Trừ hết phần còn lại vào năm cũ
        //                        if (remainOld > 0)
        //                        {
        //                            var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
        //                            {
        //                                UserId = userId,
        //                                Year = yearFrom,
        //                                DaysToDeduct = remainOld
        //                            };

        //                            var kq1 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
        //                            if (!kq1)
        //                            {
        //                                _unitOfWork.Rollback();
        //                                return ServiceResult.Failed("Có lỗi khi cập nhật phép năm cũ");
        //                            }

        //                            phepConNamCu = 0;
        //                        }

        //                        // Phần còn lại chuyển sang trừ vào năm mới
        //                        if (remainNew > 0)
        //                        {
        //                            var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
        //                            {
        //                                UserId = userId,
        //                                Year = yearFrom + 1,
        //                                DaysToDeduct = remainNew
        //                            };

        //                            var kq2 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
        //                            if (!kq2)
        //                            {
        //                                _unitOfWork.Rollback();
        //                                return ServiceResult.Failed("Có lỗi khi cập nhật phép năm mới");
        //                            }

        //                            phepConNamMoi -= remainNew;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    // Sau mốc 31/3 hoặc phép năm cũ đã hết, trừ vào phép năm mới
        //                    var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
        //                    {
        //                        UserId = userId,
        //                        Year = yearFrom + 1,
        //                        DaysToDeduct = value
        //                    };

        //                    var kq2 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
        //                    if (!kq2)
        //                    {
        //                        _unitOfWork.Rollback();
        //                        return ServiceResult.Failed("Có lỗi khi cập nhật phép năm mới");
        //                    }

        //                    phepConNamMoi -= value;
        //                }
        //            }






        //            //foreach (var (date, period) in days)
        //            //{
        //            //    double value = period == "FullDay" ? 1.0 : (period == "Morning" || period == "Afternoon" ? 0.5 : 0);

        //            //    int year = (date.Year == yearFrom && phepConNamCu1 > 0) || (date.Year == yearTo && date <= new DateTime(yearTo, 3, 31) && phepConNamCu1 > 0)
        //            //        ? yearFrom : yearTo;

        //            //    var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
        //            //    {
        //            //        UserId = userId,
        //            //        Year = year,
        //            //        DaysToDeduct = value
        //            //    };

        //            //    var kq1 = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
        //            //    if (!kq1)
        //            //    {
        //            //        _unitOfWork.Rollback();
        //            //        return ServiceResult.Failed("Có lỗi khi cập nhật phép");
        //            //    }

        //            //    // KHÔNG gọi ReturnUserLeaveBalance ở đây!
        //            //}                 



        //            //foreach (var (date, period) in days)
        //            //{
        //            //    double value = period == "FullDay" ? 1.0 : (period == "Morning" || period == "Afternoon" ? 0.5 : 0);

        //            //    if (date.Year == yearFrom && phepConNamCu1 > 0)
        //            //    {
        //            //        var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
        //            //        {
        //            //            UserId = userId,
        //            //            Year = yearFrom,
        //            //            DaysToDeduct = value

        //            //        };

        //            //        var kq = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
        //            //        if(!kq)
        //            //        {
        //            //            _unitOfWork.Rollback();
        //            //            return ServiceResult.Failed("Có lỗi khi cập nhật phép");
        //            //        }

        //            //    }
        //            //    else if (date.Year == yearTo && date <= new DateTime(2026, 3, 31) && phepConNamCu1 > 0)
        //            //    {
        //            //        var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
        //            //        {
        //            //            UserId = userId,
        //            //            Year = yearFrom,
        //            //            DaysToDeduct = value                             
        //            //        };

        //            //        var kq = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);

        //            //        if (!kq)
        //            //        {
        //            //            _unitOfWork.Rollback();
        //            //            return ServiceResult.Failed("Có lỗi khi cập nhật phép");
        //            //        }
        //            //    }
        //            //    else
        //            //    {
        //            //        var userLeaveBalance = new LeaveManagement.Domain.Entities.UserLeaveBalances
        //            //        {
        //            //            UserId = userId,
        //            //            Year = yearTo,
        //            //            DaysToDeduct = value                                
        //            //        };

        //            //        var kq = await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(userLeaveBalance);
        //            //        if (!kq)
        //            //        {
        //            //            _unitOfWork.Rollback();
        //            //            return ServiceResult.Failed("Có lỗi khi cập nhật phép");
        //            //        }
        //            //    }
        //            //}

        //            // 4. Lưu đơn nghỉ phép
        //            var entity1 = new LeaveRequest
        //            {
        //                UserId = request.UserId,
        //                FullName = request.FullName,
        //                MaChucVu = request.MaChucVu,
        //                MaPhongBan = request.MaPhongBan,
        //                TenPhongBan = request.TenPhongBan,
        //                LeaveTypeId = request.LeaveTypeId,
        //                FromDate = request.FromDate,
        //                ToDate = request.ToDate,
        //                FromDateType = request.FromDateType,
        //                ToDateType = request.ToDateType,
        //                TotalLeaveDays = totalLeaveDays,
        //                Reason = request.Reason,
        //                VacationPlace = request.VacationPlace,
        //                Status = 0,
        //                RequestedBy = _currentUser.Id.ToString(),
        //                RequestedDate = DateTime.Now
        //                //Note = $"Trừ {Math.Min(totalLeaveDays, phepConNamCu)} ngày phép {yearFrom}, {totalLeaveDays - Math.Min(totalLeaveDays, phepConNamCu)} ngày phép {yearTo}"
        //            };

        //            await _unitOfWork.LeaveRequests.CreateAsync(entity1);
        //            _unitOfWork.Commit();

        //            //return ServiceResult.SuccessResult();

        //        }

        //        else
        //        {
        //            int nextYearFrom = yearFrom + 1;
        //            int nextYearTo = yearTo;


        //            // 1. Lấy số phép còn lại mỗi năm
        //            //double soNgayDaNghiNamCu = await _unitOfWork.LeaveRequests.GetTotalLeaveDaysAsync(request.UserId, yearFrom);
        //            double soNgayDaNghiNamMoi = await _unitOfWork.LeaveRequests.GetTotalLeaveDaysAsync(request.UserId, nextYearTo);

        //            //int soNgayPhepNamCu = request.SoNgayPhepNam; // truyền qua command hoặc lấy từ service
        //            var userPhepNamMoi = await _unitOfWork.Users.GetTotalLeaveDaysByUserIdAndYear(request.UserId, yearTo); // truyền qua command hoặc lấy từ service
        //            int soNgayPhepNamMoi = userPhepNamMoi!.SoNgayPhepNam;

        //            double phepConNamCu = soNgayPhepNamCu - soNgayDaNghiNamCu;
        //            double phepConNamMoi = soNgayPhepNamMoi - soNgayDaNghiNamMoi;

        //            // 2. Tính tổng số ngày xin nghỉ (đã loại trừ ngày lễ, bù, nửa ngày...)
        //            var holidays = await _unitOfWork.LeaveRequests.GetAllHolidaysAsync(request.FromDate, request.ToDate);
        //            var compensateDays = await _unitOfWork.LeaveRequests.GetAllCompensateDayAsync(request.FromDate, request.ToDate);

        //            double totalLeaveDays = WorkingDayCalculator.GetWorkingDays(
        //                request.FromDate, request.ToDate,
        //                request.FromDateType!, request.ToDateType!,
        //                holidays, compensateDays);

        //            // 3. Xử lý nghiệp vụ:
        //            var ngay01_01NamMoi = new DateTime(yearTo, 1, 1);
        //            var ngay31_03NamMoi = new DateTime(yearTo, 3, 31);

        //            // Nếu đã hết phép năm cũ từ 1/1, chỉ dùng phép năm mới
        //            if (phepConNamCu <= 0 && request.FromDate >= ngay01_01NamMoi)
        //            {
        //                if (totalLeaveDays > phepConNamMoi)
        //                    return ServiceResult.Failed($"Bạn chỉ còn {phepConNamMoi} ngày phép năm {yearTo}, không đủ để xin nghỉ {totalLeaveDays} ngày.");

        //                var userDto = new LeaveManagement.Domain.Entities.UserLeaveBalances
        //                {
        //                    UserId = request.UserId,
        //                    Year = yearTo,
        //                    LeaveDaysTaken = totalLeaveDays
        //                };

        //                var model1 = _mapper.Map<LeaveManagement.Domain.Entities.UserLeaveBalances>(userDto);

        //                await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(model1);
        //            }
        //            else
        //            {
        //                // Vẫn còn phép năm cũ (được dùng tới 31/3 năm sau)
        //                double usedOldYear = Math.Min(totalLeaveDays, phepConNamCu);
        //                double usedNewYear = totalLeaveDays - usedOldYear;

        //                if (usedNewYear > phepConNamMoi)
        //                    return ServiceResult.Failed($"Bạn chỉ còn {phepConNamCu} ngày phép năm {yearFrom} và {phepConNamMoi} ngày phép năm {yearTo}, không đủ để xin nghỉ {totalLeaveDays} ngày.");

        //                if (usedOldYear > 0)
        //                {
        //                    var userDto = new LeaveManagement.Domain.Entities.UserLeaveBalances
        //                    {
        //                        UserId = request.UserId,
        //                        Year = yearTo,
        //                        LeaveDaysTaken = usedOldYear
        //                    };

        //                    var model1 = _mapper.Map<LeaveManagement.Domain.Entities.UserLeaveBalances>(userDto);

        //                    await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(model1);
        //                }

        //                if (usedNewYear > 0)
        //                {
        //                    var userDto = new LeaveManagement.Domain.Entities.UserLeaveBalances
        //                    {
        //                        UserId = request.UserId,
        //                        Year = yearTo,
        //                        LeaveDaysTaken = usedNewYear
        //                    };

        //                    var model1 = _mapper.Map<LeaveManagement.Domain.Entities.UserLeaveBalances>(userDto);

        //                    await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(model1);
        //                }
        //                //await _unitOfWork.UserLeaveBalances.DeductUserLeaveBalance(request.UserId, yearTo, usedNewYear);

        //                return ServiceResult.Failed("Có lỗi khi gửi duyệt: ");
        //            }
        //        }
        //        return ServiceResult.SuccessResult();               

        //    }
        //    catch (Exception ex)
        //    {
        //        _unitOfWork.Rollback();
        //        return ServiceResult.Failed("Có lỗi khi gửi duyệt: " + ex.Message);
        //    }
        //}
    }


}
