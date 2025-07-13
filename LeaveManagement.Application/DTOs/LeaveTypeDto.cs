using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.DTOs
{
    public class LeaveTypeDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int MaxDays { get; set; }
    }

}
