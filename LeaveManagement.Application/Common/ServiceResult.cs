using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Common
{
    // Application/Common/ServiceResult.cs
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
        public static ServiceResult SuccessResult() => new ServiceResult { Success = true };
        public static ServiceResult Failed(string msg) => new ServiceResult { Success = false, Message = msg };
    }
    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }

        public static ServiceResult<T> SuccessResult(T data, string? message = null)
            => new ServiceResult<T> { Success = true, Data = data, Message = message };
        public static new ServiceResult<T> Failed(string msg)
            => new ServiceResult<T> { Success = false, Message = msg };
    }
}
