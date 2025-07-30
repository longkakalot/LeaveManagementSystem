using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Interfaces
{
    // Application/Interfaces/IUserRepository.cs
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<IList<User>> FindApproverAsync(string maChucVu, string maPhongBan);

        Task<IList<User>> FindApproverAsyncByMaChucVu(string maChucVu);
        Task<User?> LoginAsync(string username, string hashedPassword);

        Task<User?> GetTotalLeaveDaysByUserIdAndYear(int userId, int year);

        // Có thể thêm tìm kiếm nâng cao hơn nếu workflow cần
    }

}
