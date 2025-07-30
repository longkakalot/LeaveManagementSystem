using AutoMapper;
using Dapper;
using LeaveManagement.Application.Authentication.Dtos;
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

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.GetAllByUserId
{    
    public class GetAllByUserIdQueryHandler : IRequestHandler<GetAllByUserIdQuery, List<LeaveRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;        
        private readonly IMapper _mapper;

        public GetAllByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {            
            _mapper = mapper;
            _unitOfWork = unitOfWork;            
        }        

        public async Task<List<LeaveRequestDto>> Handle(GetAllByUserIdQuery request, CancellationToken cancellationToken)
        {
            var flatList = await _unitOfWork.LeaveRequests.GetAllByUserId(request.UserId);

            var result = flatList
                .GroupBy(x => x.Id)
                .Select(g => new LeaveRequestDto
                {
                    Id = g.Key,
                    FullName = g.First().FullName,
                    MaChucVu = g.First().MaChucVu,
                    MaPhongBan = g.First().MaPhongBan,
                    TenPhongBan = g.First().TenPhongBan,
                    LeaveTypeId = g.First().LeaveTypeId,
                    FromDate = g.First().FromDate,
                    ToDate = g.First().ToDate,
                    Reason = g.First().Reason,
                    VacationPlace = g.First().VacationPlace,
                    Status = g.First().Status,
                    TotalLeaveDays = g.First().TotalLeaveDays,         
                    StepApprove = g.First().StepApprove,
                    StepOrder = g.First().StepOrder,
                    // ... các trường master ...
                    Details = g.Select(d => new DTOs.LeaveRequestDetailDto
                    {
                        Date = d.Date,
                        Period = d.Period,
                        Value = d.Value,
                        Year = d.YearDetail
                    }).OrderBy(d => d.Date).ThenBy(d => d.Period).ToList()
                })
                .OrderByDescending(x => x.Id)
                .ToList();            

            return result!;
        }

    }

}
