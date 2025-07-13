using AutoMapper;
using LeaveManagement.Application.Features.LeaveRequests.Commands.ApproveLeaveRequest;
using LeaveManagement.Application.Features.LeaveRequests.Commands.CancelLeaveRequest;
using LeaveManagement.Application.Features.LeaveRequests.Commands.RejectLeaveRequest;
using LeaveManagement.Application.Features.LeaveRequests.Queries.GetMyPendingApproval;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Enums;
using LeaveManagement.WebUI.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.WebUI.Controllers
{
    public class LeaveApprovalRequestController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public LeaveApprovalRequestController(IMediator mediator, ICurrentUserService currentUser, IMapper mapper)
        {
            _mediator = mediator;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        // Trang danh sách các phiếu chờ duyệt
        [Authorize]
        public async Task<IActionResult> Index(string tab = "Pending", int page = 1, int pageSize = 6)
        {
            var approverId = _currentUser.UserId;

            // Giả sử GetMyApprovalsQuery trả về tất cả đơn mà user là người duyệt (Pending + Processed)
            var allApprovals = await _mediator.Send(new GetMyPendingApprovalsQuery { ApproverUserId = approverId });
            var approvalDtos = _mapper.Map<List<LeaveApprovalRequestDto>>(allApprovals);

            var pending = approvalDtos.Where(x => x.Status == LeaveApprovalStatus.Pending).ToList();
            var approved = approvalDtos.Where(x => x.Status == LeaveApprovalStatus.Approved).ToList();
            var rejected = approvalDtos.Where(x => x.Status == LeaveApprovalStatus.Rejected).ToList();

            var model = new LeaveApprovalTabViewModel
            {
                Pending = pending.Skip((tab == "Pending" ? (page - 1) * pageSize : 0)).Take(tab == "Pending" ? pageSize : int.MaxValue).ToList(),
                PendingTotal = pending.Count,
                Approved = approved.Skip((tab == "Approved" ? (page - 1) * pageSize : 0)).Take(tab == "Approved" ? pageSize : int.MaxValue).ToList(),
                ApprovedTotal = approved.Count,
                Rejected = rejected.Skip((tab == "Rejected" ? (page - 1) * pageSize : 0)).Take(tab == "Rejected" ? pageSize : int.MaxValue).ToList(),
                RejectedTotal = rejected.Count,
                PageIndex = page,
                PageSize = pageSize,
                CurrentTab = tab
            };

            return View(model);
        }


        // Xử lý duyệt phiếu (POST)
        [HttpPost]
        public async Task<IActionResult> Approve(int id, string comment)
        {
            var approverId = _currentUser.UserId;
            var result = await _mediator.Send(new ApproveLeaveRequestCommand
            {
                LeaveApprovalRequestId = id,
                ApproverId = approverId,
                Comment = comment
            });

            TempData[result.Success ? "Success" : "Error"] = result.Message ?? (result.Success ? "Duyệt thành công!" : "Lỗi duyệt!");
            return RedirectToAction("Index");
        }

        // (Nếu muốn xử lý từ chối, thêm action này)
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string comment)
        {
            var approverId = _currentUser.UserId;
            var result = await _mediator.Send(new RejectLeaveRequestCommand
            {
                LeaveApprovalRequestId = id,
                ApproverId = approverId,
                Comment = comment
            });

            TempData[result.Success ? "Success" : "Error"] = result.Message ?? (result.Success ? "Từ chối thành công!" : "Lỗi từ chối!");
            return RedirectToAction("Index");
        }

        // (Nếu muốn xử lý hủy, thêm action này)
        [HttpPost]
        public async Task<IActionResult> Cancel(int id, string comment)
        {
            var approverId = _currentUser.UserId;
            var result = await _mediator.Send(new CancelLeaveRequestCommand
            {
                LeaveApprovalRequestId = id,
                ApproverId = approverId,
                Comment = comment
            });

            TempData[result.Success ? "Success" : "Error"] = result.Message ?? (result.Success ? "Từ chối thành công!" : "Lỗi từ chối!");
            return RedirectToAction("Index");
        }
    }

}
