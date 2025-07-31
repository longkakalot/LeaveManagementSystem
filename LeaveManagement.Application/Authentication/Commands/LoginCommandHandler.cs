using AutoMapper;
using Dapper;
using LeaveManagement.Application.Authentication.Dtos;
using LeaveManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Authentication.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtService;
        private readonly IMapper _mapper;

        public LoginCommandHandler(IUnitOfWork unitOfWork,
            IMapper mapper,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtService = jwtTokenService;
            _mapper = mapper;
        }


        //private readonly IDbConnection _dbConnection;        

        //private readonly IPasswordHasher _passwordHasher;
        //private readonly IJwtTokenService _jwtService;

        //public LoginCommandHandler(IDbConnection dbConnection,
        //    IPasswordHasher passwordHasher,
        //    IJwtTokenService jwtService)
        //{
        //    _dbConnection = dbConnection;
        //    _passwordHasher = passwordHasher;
        //    _jwtService = jwtService;
        //}

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var hashedPassword = _passwordHasher.HashPassword(request.Password!);

                var user = await _unitOfWork.Users.LoginAsync(request.Username!, hashedPassword);

                if (user == null)
                    throw new UnauthorizedAccessException("Invalid username or password");

                var userDto = _mapper.Map<UserDto>(user);

                return _jwtService.GenerateToken(userDto);
                
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                throw;
            }
            
        }
    }
}

