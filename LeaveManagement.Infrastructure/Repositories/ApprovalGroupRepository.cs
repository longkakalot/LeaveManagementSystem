using Dapper;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Infrastructure.Repositories
{
    //public class ApprovalGroupRepository : IApprovalGroupRepository
    //{
    //    private readonly IDbConnectionFactory _connectionFactory;

    //    public ApprovalGroupRepository(IDbConnectionFactory connectionFactory)
    //    {
    //        _connectionFactory = connectionFactory;
    //    }
    //    public Task<List<ApprovalGroup>> GetByRoleAndConditionAsync(string maChucVu, int days)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class ApprovalGroupRepository : IApprovalGroupRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        // Constructor mới: nhận connection và transaction từ UnitOfWork
        public ApprovalGroupRepository(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        /// <summary>
        /// Lấy danh sách nhóm duyệt theo chức vụ và số ngày nghỉ (ví dụ).
        /// </summary>
        public async Task<List<ApprovalGroup>> GetByRoleAndConditionAsync(string maChucVu, int days)
        {
            // Ví dụ truy vấn lấy ra nhóm phù hợp (bạn tự điều chỉnh theo logic thực tế)
            var sql = @"
            SELECT *
            FROM ApprovalGroup
            WHERE RoleNguoiGui = @MaChucVu
            -- Nếu có điều kiện về số ngày nghỉ, thêm vào WHERE
        ";

            var result = await _connection.QueryAsync<ApprovalGroup>(
                sql,
                new { MaChucVu = maChucVu },
                transaction: _transaction // <-- Nên truyền transaction cho chuẩn, dù chỉ là query
            );
            return result.ToList();
        }

        // Nếu có method Add/Update, cũng truyền _transaction vào các lệnh Dapper
    }

}
