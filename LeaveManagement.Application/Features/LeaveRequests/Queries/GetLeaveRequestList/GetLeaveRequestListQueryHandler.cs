using AutoMapper;
using Dapper;
using LeaveManagement.Application.DTOs;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.GetLeaveRequestList
{
    //public class GetLeaveRequestListQueryHandler : IRequestHandler<GetLeaveRequestListQuery, List<LeaveRequestDto>>
    //{
    //    private readonly IDbConnectionFactory _connectionFactory;
    //    private readonly IMapper _mapper;


    //    public GetLeaveRequestListQueryHandler(IDbConnectionFactory connectionFactory, IMapper mapper)
    //    {
    //        _connectionFactory = connectionFactory;
    //        _mapper = mapper;
    //    }

    //    public async Task<List<LeaveRequestDto>> Handle(GetLeaveRequestListQuery request, CancellationToken cancellationToken)
    //    {
    //        using var _connection = _connectionFactory.CreateCommandConnection();

    //        var result = await _connection.QueryAsync<LeaveRequest>(
    //            "sp_LEAVEREQUESTS",
    //            new { UserId = request.UserId, Action = "GetDataByUserId" },
    //            commandType: CommandType.StoredProcedure);

    //        var dtos = _mapper.Map<List<LeaveRequestDto>>(result);


    //        //// TEMP LOG để test
    //        //foreach (var dto in dtos)
    //        //{
    //        //    Console.WriteLine($"ID: {dto.Id}, StatusLabel: {dto.StatusLabel}");
    //        //}

    //        return _mapper.Map<List<LeaveRequestDto>>(result);

    //        //return result.ToList();


    //    }
    //}
    public class GetLeaveRequestListQueryHandler : IRequestHandler<GetLeaveRequestListQuery, List<LeaveRequestDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper _mapper;

        public GetLeaveRequestListQueryHandler(IDbConnectionFactory connectionFactory, IMapper mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public async Task<List<LeaveRequestDto>> Handle(GetLeaveRequestListQuery request, CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateCommandConnection();

            var result = await connection.QueryAsync<LeaveRequest>(
                "sp_LEAVEREQUESTS",
                new { UserId = request.UserId, Action = "GetDataByUserId" },
                commandType: CommandType.StoredProcedure);

            return _mapper.Map<List<LeaveRequestDto>>(result);
        }
    }

}
