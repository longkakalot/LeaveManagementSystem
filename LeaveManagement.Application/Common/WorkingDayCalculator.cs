using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Common
{
    public static class WorkingDayCalculator
    {
        //public static bool IsContinuousLeave(
        //    DateTime fromDate,
        //    DateTime toDate,
        //    List<DateTime> holidays,
        //    List<DateTime> compensateWorkingDays)
        //{
        //    for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
        //    {
        //        bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        //        bool isHoliday = holidays.Contains(date);
        //        bool isCompensateWork = compensateWorkingDays.Contains(date);

        //        if (!isWeekend && !isHoliday && !isCompensateWork)
        //        {
        //            // Đây là ngày làm việc (không thuộc chuỗi nghỉ liên tục)
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        public static bool IsWorkingDay(
            DateTime date,
            List<DateTime> holidays,
            List<DateTime> compensateWorkingDays)
        {
            if (compensateWorkingDays.Contains(date.Date))
                return true;
            if (holidays.Contains(date.Date))
                return false;
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                return false;
            return true;
        }


        //Kiểm tra có ít nhất 1 ngày làm việc
        public static bool HasAtLeastOneWorkingDay(
            DateTime fromDate, DateTime toDate,
            List<DateTime> holidays, List<DateTime> compensateWorkingDays)
        {
            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                bool isCompensateWork = compensateWorkingDays.Contains(date);
                bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
                bool isHoliday = holidays.Contains(date);

                // Nếu là ngày làm việc thực sự
                if (isCompensateWork || (!isWeekend && !isHoliday))
                    return true;
            }
            return false;
        }


        public static double GetWorkingDays(
            DateTime fromDate,
            DateTime toDate,
            string fromDateType, // "Full", "Morning", "Afternoon"
            string toDateType,   // "Full", "Morning", "Afternoon"
            List<DateTime> holidays,
            List<DateTime> compensateWorkingDays)
        {
            double total = 0;
            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                // Nếu là ngày làm bù (trùng T7/CN), vẫn tính là ngày làm việc!
                bool isCompensateWork = compensateWorkingDays.Contains(date);
                bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
                bool isHoliday = holidays.Contains(date);

                // Ngày làm việc thực sự: 
                //  - Là ngày làm bù
                //  - Hoặc KHÔNG phải cuối tuần, KHÔNG phải ngày lễ
                bool isWorking = isCompensateWork || (!isWeekend && !isHoliday);

                if (!isWorking)
                    continue;

                // Xử lý nửa ngày
                if (date == fromDate.Date && date == toDate.Date)
                {
                    if (fromDateType == "Full")
                        total += 1;
                    else
                        total += 0.5;
                }
                else if (date == fromDate.Date)
                {
                    total += fromDateType == "Full" ? 1 : 0.5;
                }
                else if (date == toDate.Date)
                {
                    total += toDateType == "Full" ? 1 : 0.5;
                }
                else
                {
                    total += 1;
                }
            }
            return total;
        }


        //Kiểm tra tất cả ngày xin nghỉ phải là ngày làm việc
        public static bool IsAllWorkingDays(
            DateTime fromDate,
            DateTime toDate,
            List<DateTime> holidays,
            List<DateTime> compensateWorkingDays)
        {
            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                bool isCompensateWork = compensateWorkingDays.Contains(date.Date);
                bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
                bool isHoliday = holidays.Contains(date.Date);

                // Nếu KHÔNG phải ngày làm việc thực sự
                if (!isCompensateWork && (isWeekend || isHoliday))
                    return false; // Phát hiện ra 1 ngày không hợp lệ => cấm tạo đơn luôn
            }
            return true; // Tất cả ngày đều hợp lệ (chỉ toàn ngày làm việc)
        }


        /// <summary>
        /// Trả về danh sách tất cả các ngày nghỉ của user trong một kỳ nghỉ,
        /// với thông tin nghỉ buổi sáng/chiều/cả ngày.
        /// </summary>
        public static List<(DateTime Date, string Period)> GetAllWorkingDatesWithType(
            DateTime fromDate,
            DateTime toDate,
            string fromDateType, // "FullDay", "Morning", "Afternoon"
            string toDateType,
            List<DateTime> holidays,
            List<DateTime> compensateDays)
        {
            var result = new List<(DateTime, string)>();
            var holidaySet = new HashSet<DateTime>(holidays.Select(d => d.Date));
            var compensateSet = new HashSet<DateTime>(compensateDays.Select(d => d.Date));

            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                // Loại trừ ngày nghỉ cuối tuần, ngày lễ (trừ khi là ngày bù)
                bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
                bool isHoliday = holidaySet.Contains(date);
                bool isCompensate = compensateSet.Contains(date);

                if ((isWeekend || isHoliday) && !isCompensate)
                    continue;

                // 1. Nếu là ngày duy nhất của đơn nghỉ (từ == đến)
                if (fromDate.Date == toDate.Date)
                {
                    if (fromDateType == "Morning" && toDateType == "Full")
                        result.Add((date, "Morning"));
                    else if (fromDateType == "Afternoon" && toDateType == "Full")
                        result.Add((date, "Afternoon"));
                    else if (fromDateType == "Morning" && toDateType == "Afternoon")
                    {
                        result.Add((date, "Morning"));
                        result.Add((date, "Afternoon"));
                    }
                    else
                        result.Add((date, "FullDay"));
                }
                // 2. Ngày đầu kỳ nghỉ
                else if (date == fromDate.Date)
                {
                    if (fromDateType == "Morning")
                        result.Add((date, "Morning"));
                    else if (fromDateType == "Afternoon")
                        result.Add((date, "Afternoon"));
                    else
                        result.Add((date, "FullDay"));
                }
                // 3. Ngày cuối kỳ nghỉ
                else if (date == toDate.Date)
                {
                    if (toDateType == "Morning")
                        result.Add((date, "Morning"));
                    else if (toDateType == "Afternoon")
                        result.Add((date, "Afternoon"));
                    else
                        result.Add((date, "FullDay"));
                }
                // 4. Các ngày ở giữa luôn là cả ngày
                else
                {
                    result.Add((date, "FullDay"));
                }
            }

            return result;
        }


        /// <summary>
        /// Lấy tất cả các ngày user đã xin nghỉ (kể cả buổi) từ các LeaveRequest còn hiệu lực (không tính đã hủy/reject)
        /// </summary>
        public static List<(DateTime Date, string Period)> GetAllUserUsedLeaveDates(
            IEnumerable<LeaveRequest> leaveRequests,
            List<DateTime> holidays,
            List<DateTime> compensateDays)
        {
            var usedDates = new List<(DateTime, string)>();

            foreach (var req in leaveRequests)
            {
                var days = WorkingDayCalculator.GetAllWorkingDatesWithType(
                    req.FromDate,
                    req.ToDate,
                    req.FromDateType!,
                    req.ToDateType!,
                    holidays,
                    compensateDays
                );
                usedDates.AddRange(days);
            }

            return usedDates
                .Distinct()
                .OrderBy(x => x.Item1)
                .ThenBy(x => x.Item2)
                .ToList();
        }
    }
}
