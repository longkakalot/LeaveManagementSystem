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
{// Infrastructure/Persistence/Repositories/ApprovalStepRepository.cs
 //public class ApprovalStepRepository : IApprovalStepRepository
 //{
 //    private readonly IDbConnectionFactory _connectionFactory;        

    //    public ApprovalStepRepository(IDbConnectionFactory connectionFactory)
    //    {
    //        _connectionFactory = connectionFactory;
    //    }

    //    public async Task<List<ApprovalStep>> GetStepsByGroupAsync(string maChucVu, int days)
    //    {
    //        try
    //        {
    //            using var connection = _connectionFactory.CreateCommandConnection();

    //            var approvalStepList = new List<ApprovalStep>();

    //            var sql = $@"SELECT s.StepOrder, s.ApproverRole, s.OnlyOver5Days
    //            FROM ApprovalGroups g
    //            JOIN ApprovalSteps s ON g.Id = s.GroupId
    //            WHERE (
    //                ',' + g.MaChucVu + ',' LIKE '%,' + '{maChucVu}' + ',%'
    //            )
    //            AND (({days} > 5 AND s.OnlyOver5Days = 1)
    //                 OR ({days} <= 5 AND s.OnlyOver5Days = 0))
    //            ORDER BY s.StepOrder;";

    //            var kq1 = await connection.QueryAsync<ApprovalStep>(sql, param: null, commandType: CommandType.Text);
    //            var kq = kq1.ToList();
    //            return kq;
    //        }
    //        catch (Exception ex)
    //        {
    //            var a = ex.Message;
    //            throw;
    //        }

    //    }
    //}

    public class ApprovalStepRepository : IApprovalStepRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        // Constructor mới: nhận connection và transaction từ UnitOfWork
        public ApprovalStepRepository(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        /// <summary>
        /// Lấy các bước duyệt (step) theo mã chức vụ và số ngày nghỉ.
        /// </summary>
        public async Task<List<ApprovalStep>> GetStepsByGroupAsync(string maChucVu, int days)
        {
            var sql = @"
            SELECT s.StepOrder, s.ApproverRole, s.OnlyOver5Days
            FROM ApprovalGroups g
            JOIN ApprovalSteps s ON g.Id = s.GroupId
            WHERE (
                ',' + g.MaChucVu + ',' LIKE '%,' + @MaChucVu + ',%'
            )
            AND ((@Days > 5 AND s.OnlyOver5Days = 1)
                 OR (@Days <= 5 AND s.OnlyOver5Days = 0))
            ORDER BY s.StepOrder;";

            var param = new { MaChucVu = maChucVu, Days = days };

            var result = await _connection.QueryAsync<ApprovalStep>(
                sql,
                param,
                transaction: _transaction  // Truyền transaction cho đúng UoW
            );

            return result.ToList();
        }
    }


}
