using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    public interface ILeaveRequestDetailRepository
    {
        // Thêm 1 hoặc nhiều dòng detail (insert)
        //Task<int> AddAsync(LeaveRequestDetail detail);
        Task<int> AddRangeAsync(IEnumerable<LeaveRequestDetail> details);

        // Lấy toàn bộ detail của 1 đơn nghỉ phép
        Task<List<LeaveRequestDetail>> GetByLeaveRequestId(int leaveRequestId);

        // Xóa toàn bộ detail của 1 đơn nghỉ phép
        Task<bool> DeleteByLeaveRequestId(int leaveRequestId);

        // (Tùy chọn) Xóa từng dòng detail (ít dùng)
        //Task<bool> DeleteAsync(int detailId);

        // (Tùy chọn) Lấy detail theo điều kiện khác, ví dụ theo user hoặc ngày
        Task<List<LeaveRequestDetail>> GetByUserId(int userId);
        Task<List<LeaveRequestDetail>> GetByDate(DateTime date);
    }
}
