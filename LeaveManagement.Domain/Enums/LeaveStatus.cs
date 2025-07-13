using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Domain.Enums
{
    public enum LeaveStatus
    {
        Pending = 0,      // Chưa gửi duyệt
        Submitted = 1,    // Đã gửi duyệt (chờ phê duyệt)
        Approved = 2,     // Đã duyệt
        Rejected = 3      // Bị từ chối
    }
}