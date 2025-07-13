using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    public interface IHolidayRepository
    {
        Task<List<DateTime>> GetHolidaysInRange(DateTime fromDate, DateTime toDate);
    }
}
