using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Domain.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Controller { get; set; } = default!;
        public string Action { get; set; } = default!;
        public string? Description { get; set; }
    }
}
