using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Domain.Entities
{
    // Domain/Entities/ApprovalGroup.cs
    public class ApprovalGroup
    {
        public int Id { get; set; }
        public string? GroupCode { get; set; }
        public string? GroupName { get; set; }
        public string? MaChucVu { get; set; }
    }

}
