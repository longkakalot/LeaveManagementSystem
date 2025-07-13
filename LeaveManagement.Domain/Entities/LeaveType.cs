using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Domain.Entities
{
    public class LeaveType
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int MaxDays { get; set; }
    }
}
