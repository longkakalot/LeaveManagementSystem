using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequests.Commands.DeleteLeaveRequest
{
    //public class DeleteLeaveRequestCommandHandler : IRequestHandler<DeleteLeaveRequestCommand, bool>
    //{
    //    private readonly ILeaveRequestRepository _repository;

    //    public DeleteLeaveRequestCommandHandler(ILeaveRequestRepository repository)
    //    {
    //        _repository = repository;
    //    }

    //    public async Task<bool> Handle(DeleteLeaveRequestCommand request, CancellationToken cancellationToken)
    //    {
    //        var leaveRequest = await _repository.GetByIdAsync(request.Id);
    //        if (leaveRequest == null || leaveRequest.Status != (int)LeaveStatus.Pending)
    //            return false;

    //        return await _repository.DeleteAsync(request.Id);
    //    }
    //}
    public class DeleteLeaveRequestCommandHandler : IRequestHandler<DeleteLeaveRequestCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLeaveRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(request.Id);
                if (leaveRequest == null || leaveRequest.Status != (int)LeaveStatus.Pending)
                    return false;

                var success = await _unitOfWork.LeaveRequests.DeleteAsync(request.Id);

                if (success)
                    _unitOfWork.Commit();
                else
                    _unitOfWork.Rollback();

                return success;
            }
            catch
            {
                _unitOfWork.Rollback();
                return false;
            }
        }
    }

}
