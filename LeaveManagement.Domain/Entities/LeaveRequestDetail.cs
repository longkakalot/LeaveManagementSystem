using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Domain.Entities
{
    public class LeaveRequestDetail
    {
        public int Id { get; set; }
        public int LeaveRequestId { get; set; }
        public DateTime? Date { get; set; }
        public string? Period { get; set; }
        public int Year { get; set; }
        public double Value { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
