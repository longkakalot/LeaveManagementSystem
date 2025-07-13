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
        private readonly ILeaveApprovalRequestRepository _repo;
        private readonly IMapper _mapper;

        public GetMyPendingApprovalsQueryHandler(ILeaveApprovalRequestRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<LeaveApprovalRequestDto>> Handle(GetMyPendingApprovalsQuery request, CancellationToken cancellationToken)
        {
            var result = await _repo.GetPendingByApproverAsync(request.ApproverUserId);
            return _mapper.Map<List<LeaveApprovalRequestDto>>(result);
        }
    }

}
