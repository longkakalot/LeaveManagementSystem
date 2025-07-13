using LeaveManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Infrastructure.Helpers
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string plainText)
        {
            return Encrypt.Encrypt_String.Encrypt_Password(plainText); // hoặc tên hàm đúng từ DLL
        }
    }
}
