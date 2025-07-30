using AutoMapper;
using LeaveManagement.Application.Common;
using LeaveManagement.Application.DTOs;
using LeaveManagement.Application.Features.LeaveRequests.Commands.ApproveLeaveRequest;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace LeaveManagement.Application.Features.UserLeaveBalances.Commands.AddUserLeaveBalance
{
    public class AddUserLeaveBalanceCommandHandler : IRequestHandler<AddUserLeaveBalanceCommand, ServiceResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        

        public AddUserLeaveBalanceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }

        public async Task<ServiceResult> Handle(AddUserLeaveBalanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var model = new LeaveManagement.Domain.Entities.UserLeaveBalances
                {
                    UserId = request.UserId,
                    Year = request.Year,
                    LeaveDaysGranted = request.LeaveDaysGranted,
                    LeaveDaysRemain = request.LeaveDaysRemain,
                    LeaveDaysTaken = request.LeaveDaysTaken,
                    DaysToDeduct = request.DaysToDeduct,
                    DaysToReturn = request.DaysToReturn
                };

                await _unitOfWork.UserLeaveBalances.AddUserLeaveBalance(model);                

                _unitOfWork.Commit();
                return ServiceResult.SuccessResult();
            }
            catch (Exception ex)
            {                
                _unitOfWork.Rollback();
                return ServiceResult.Failed("Có lỗi thêm UserLeaveBalances: " + ex.Message);
            }            
        }
    }
}
