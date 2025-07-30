using LeaveManagement.Application.DTOs;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.GetLeaveTypes
{   

    public class GetLeaveTypesQueryHandler : IRequestHandler<GetLeaveTypesQuery, List<LeaveType>>
    {
        private readonly ILeaveTypeRepository _leaveTypeRepository;

        public GetLeaveTypesQueryHandler(ILeaveTypeRepository leaveTypeRepository)
        {
            _leaveTypeRepository = leaveTypeRepository;
        }

        public async Task<List<LeaveType>> Handle(GetLeaveTypesQuery request, CancellationToken cancellationToken)
        {
            // Gọi repo chỉ đọc, không cần transaction, chuẩn best practice
            var leaveTypes = await _leaveTypeRepository.GetAllAsync();

            // Có thể trả về luôn nếu đã đúng dạng DTO
            return leaveTypes.Select(lt => new LeaveType
            {
                Id = lt.Id,
                Name = lt.Name
            }).ToList();
        }
    }

}
