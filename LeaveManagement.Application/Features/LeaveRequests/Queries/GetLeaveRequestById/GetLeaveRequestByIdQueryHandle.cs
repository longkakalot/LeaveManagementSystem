using Dapper;
using LeaveManagement.Application.DTOs;
using LeaveManagement.Application.Features.LeaveRequests.Queries.GetLeaveRequestList;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.GetLeaveRequestById
{   

    public class GetLeaveRequestByIdQueryHandler : IRequestHandler<GetLeaveRequestByIdQuery, LeaveRequestDto>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GetLeaveRequestByIdQueryHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<LeaveRequestDto> Handle(GetLeaveRequestByIdQuery request, CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateCommandConnection();

            var result = await connection.QuerySingleOrDefaultAsync<LeaveRequestDto>(
                "sp_LEAVEREQUESTS",
                new { Id = request.Id, Action = "GetData_ByKey" },
                commandType: CommandType.StoredProcedure);

            return result!;
        }
    }

}
