using LeaveManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Infrastructure.Repositories
{
    public class ApproverService : IApproverService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ApproverService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<int> GetApproverIdAsync(int employeeId, int level)
        {
            using var connection = _connectionFactory.CreateCommandConnection();


            //Tìm ra level của userId
            

            // Nếu Level 1 thì người duyệt sẽ là trưởng khoa
            //if (level == 1)
            //
            //    return employee.SupervisorId ?? throw new Exception("No level 1 approver");


            //Nếu level 2 thì người duyệt là BGD
            //if (level == 2)
            //{
            //    // logic tìm cấp 2...
            //}

            throw new NotImplementedException();
            
        }
    }

}
