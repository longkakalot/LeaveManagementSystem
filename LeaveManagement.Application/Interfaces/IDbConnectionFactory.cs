using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateCommandConnection();
        IDbConnection CreateQueryConnection();
    }
}
