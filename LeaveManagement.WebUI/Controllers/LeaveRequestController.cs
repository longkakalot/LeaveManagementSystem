
using AutoMapper;
using LeaveManagement.Application.Features.LeaveRequests.Commands.CreateLeaveRequest;
using LeaveManagement.Application.Features.LeaveRequests.Commands.DeleteLeaveRequest;
using LeaveManagement.Application.Features.LeaveRequests.Commands.EditLeaveRequest;
using LeaveManagement.Application.Features.LeaveRequests.Commands.SubmitLeaveRequest;
using LeaveManagement.Application.Features.LeaveRequests.Queries.GetLeaveRequestById;
using LeaveManagement.Application.Features.LeaveRequests.Queries.GetLeaveRequestList;
using LeaveManagement.Application.Features.LeaveRequests.Queries.GetLeaveTypes;
using LeaveManagement.Application.Features.LeaveRequests.Queries.GetTotalLeaveDays;
using LeaveManagement.Application.Features.LeaveRequests.Queries.ValidateLeaveRequest;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.WebUI.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace LeaveManagement.WebUI.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public LeaveRequestController(IMediator mediator, ICurrentUserService currentUserService, IMapper mapper)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = _currentUserService.UserId;
            var year = DateTime.Now.Year;

            var leaveTypes = await _mediator.Send(new GetLeaveTypesQuery());

            var soNgayDaNghi = await _mediator.Send(new GetTotalLeaveDaysQueryByUser { UserId = userId, Year = year});

            var model = new CreateLeaveRequestViewModel
            {
                LeaveTypes = leaveTypes.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList(),
                FromDate = DateTime.Today,
                ToDate = DateTime.Today,
                FromDateType = "Full",
                ToDateType = "Full",
                SoNgayPhepNam = _currentUserService.SoNgayPhepNam,
                SoNgayNghiCoBan = _currentUserService.SoNgayNghiCoBan,
                NgayPhepCongThem = _currentUserService.NgayPhepCongThem,
                TotalLeaveDays = soNgayDaNghi
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLeaveRequestViewModel model)
        {
            var userId = Convert.ToInt32(_currentUserService.UserId);
            var maChucVu = string.IsNullOrEmpty(_currentUserService.MaChucVu) ? "NV" : _currentUserService.MaChucVu;
            var maPhongBan = _currentUserService.MaPhongBan;
            var tenPhongBan = _currentUserService.TenPhongBan;
            var fullname = _currentUserService.FullName;
            var ngayPhepCongThem = _currentUserService.NgayPhepCongThem;
            var soNgayNghiCoBan = _currentUserService.SoNgayNghiCoBan;
            var soNgayPhepNam = _currentUserService.SoNgayPhepNam;
            var year = DateTime.Now.Year;

            // Lấy số ngày đã nghỉ
            var totalLeaveDays = await _mediator.Send(new GetTotalLeaveDaysQueryByUser { UserId = userId, Year = year });

            // Nạp lại select list loại phép
            async Task SetDropdownsAndData()
            {
                var leaveTypes = await _mediator.Send(new GetLeaveTypesQuery());
                model.LeaveTypes = leaveTypes.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();

                // Luôn nạp lại các trường liên quan user (nếu View hiển thị)
                model.SoNgayPhepNam = soNgayPhepNam;
                model.SoNgayNghiCoBan = soNgayNghiCoBan;
                model.NgayPhepCongThem = ngayPhepCongThem;
                model.TotalLeaveDays = totalLeaveDays;
            }

            // 1. Validate ModelState (bắt buộc giữ lại dữ liệu đã nhập)
            if (!ModelState.IsValid)
            {
                await SetDropdownsAndData();
                return View(model);
            }

            // Nếu nghỉ 1 ngày thì chỉ cần dùng FromDateType, ToDateType bỏ qua (cho đồng nhất form, nếu UI đã xử lý thì có thể bỏ dòng này)
            if (model.FromDate == model.ToDate)
            {
                // Nếu UI để 2 dropdown thì vẫn giữ, còn nếu chỉ có 1 thì sửa lại model cho đúng
                // model.ToDateType = null;
            }

            // 2. Validate nghỉ phép (chặn lỗi nghiệp vụ: ngày nghỉ không hợp lệ, hết phép, v.v.)
            var validateResult = await _mediator.Send(new ValidateLeaveRequest
            {
                UserId = userId,
                FromDate = model.FromDate,
                FromDateType = model.FromDateType,
                ToDate = model.ToDate,
                ToDateType = model.ToDateType,
                LeaveTypeId = model.LeaveTypeId
            });

            if (!validateResult.Success)
            {
                ModelState.AddModelError("", validateResult.Message!);
                await SetDropdownsAndData();
                return View(model);
            }

            // 3. Gửi lệnh tạo đơn
            var result = await _mediator.Send(new CreateLeaveRequestCommand
            {
                UserId = userId,
                FullName = fullname,
                MaChucVu = maChucVu,
                MaPhongBan = maPhongBan,
                TenPhongBan = tenPhongBan,
                LeaveTypeId = model.LeaveTypeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                FromDateType = model.FromDateType,
                ToDateType = model.ToDateType,
                Reason = model.Reason,
                VacationPlace = model.VacationPlace,
                SoNgayPhepNam = soNgayPhepNam,
                SoNgayNghiCoBan = soNgayNghiCoBan,
                NgayPhepCongThem = ngayPhepCongThem,
                TotalLeaveDays = totalLeaveDays
            });

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message!);
                await SetDropdownsAndData();
                return View(model);
            }

            // Thành công, chuyển về Index
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Index(string tab = "Drafts", int page = 1, int pageSize = 6)
        {
            var userId = Convert.ToInt32(_currentUserService.UserId);

            var maChucVu = _currentUserService.MaChucVu;
            var leaveRequestsAll = await _mediator.Send(new GetLeaveRequestListQuery { UserId = userId });

            var dtos = _mapper.Map<List<LeaveRequestDto>>(leaveRequestsAll);

            var drafts = dtos.Where(x => x.Status == LeaveStatus.Pending).ToList();
            var submitted = dtos.Where(x => x.Status == LeaveStatus.Submitted).ToList();
            var approved = dtos.Where(x => x.Status == LeaveStatus.Approved).ToList();
            var rejected = dtos.Where(x => x.Status == LeaveStatus.Rejected).ToList();

            var model = new LeaveRequestTabViewModel
            {
                Drafts = drafts.Skip((tab == "Drafts" ? (page - 1) * pageSize : 0)).Take(tab == "Drafts" ? pageSize : int.MaxValue).ToList(),
                DraftTotal = drafts.Count,
                Submitted = submitted.Skip((tab == "Submitted" ? (page - 1) * pageSize : 0)).Take(tab == "Submitted" ? pageSize : int.MaxValue).ToList(),
                SubmittedTotal = submitted.Count,
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var leaveRequest = await _mediator.Send(new GetLeaveRequestByIdQuery { Id = id });
            if (leaveRequest == null || leaveRequest.Status == LeaveStatus.Approved)
                return RedirectToAction("Index");

            var viewModel = _mapper.Map<LeaveRequestDto>(leaveRequest); // App → Web

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditLeaveRequestCommand model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var command = _mapper.Map<EditLeaveRequestCommand>(model); // giữ lại UserId
            await _mediator.Send(command);

            return RedirectToAction("Index");




            //var result = await _mediator.Send(command);
            //if (!result)
            //    return BadRequest("Không thể sửa đơn.");

            //return RedirectToAction("Index");
        }

        //// GET: LeaveRequest/Submit/5
        //public async Task<IActionResult> Submit(int id)
        //{
        //    var leaveRequest = await _mediator.Send(new GetLeaveRequestByIdQuery { Id = id});

        //    if (leaveRequest == null) return NotFound();

        //    var model = new SubmitLeaveRequestCommand
        //    {
        //        LeaveRequestId = leaveRequest.Id,
        //        FromDate = leaveRequest.FromDate,
        //        ToDate = leaveRequest.ToDate,
        //        Reason = leaveRequest.Reason,
        //        Status = leaveRequest.Status
        //    };

        //    return View(model);
        //}

        [HttpPost]
        public async Task<IActionResult> Submit(int id)
        {
            var command = new SubmitLeaveRequestCommand { LeaveRequestId = id, UserId = Convert.ToInt32(_currentUserService.UserId) };
            var result = await _mediator.Send(command); // hoặc gọi qua service layer
            if (!result.Success)
                return View("Error", result.Message);
            return RedirectToAction("Index");

            //var leaveRequest = await _mediator.Send(new GetLeaveRequestByIdQuery { Id = id });
            //await _mediator.Send(new SubmitLeaveRequestCommand { LeaveRequestId = id });
            //return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteLeaveRequestCommand { Id = id });
            if (!result)
                return BadRequest("Không thể hủy đơn.");

            return RedirectToAction("Index");
        }


        public IActionResult Success()
        {
            return View();
        }
    }
}
