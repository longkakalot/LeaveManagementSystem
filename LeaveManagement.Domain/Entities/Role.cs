using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
