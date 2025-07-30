using AutoMapper;
using LeaveManagement.Application.DTOs;
using LeaveManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Queries.GetMyPendingApproval
{
    public class GetMyPendingApprovalsQueryHandler : IRequestHandler<GetMyPendingApprovalsQuery, List<LeaveApprovalRequestDto>>
    {
        //private readonly ILeaveApprovalRequestRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetMyPendingApprovalsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<LeaveApprovalRequestDto>> Handle(GetMyPendingApprovalsQuery request, CancellationToken cancellationToken)
        {
            var flatList = await _unitOfWork.LeaveApprovalRequests.GetPendingByApproverAsync(request.ApproverUserId);

            var result = flatList
                .GroupBy(x => x.Id)
                .Select(g => new LeaveApprovalRequestDto
                {
                    Id = g.Key,
                    RequesterName = g.First().RequesterName,
                    MaChucVu = g.First().MaChucVu,
                    MaPhongBan = g.First().MaPhongBan,
                    TenPhongBan = g.First().TenPhongBan,
                    LeaveTypeId = g.First().LeaveTypeId,
                    FromDate = g.First().FromDate,
                    ToDate = g.First().ToDate,
                    Reason = g.First().Reason,
                    VacationPlace = g.First().VacationPlace,
                    //Status = g.First().Status,
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


            //return _mapper.Map<List<LeaveApprovalRequestDto>>(result);
            return result!;
        }
    }

}
