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


                //         using var connection = _connectionFactory.CreateQueryConnection();      								

                //         var sql = $@"select us.User_Id as UserId, us.User_Code as Username
                //                     ,us.User_Name as Fullname
                //, pb.MaPhongBan,pb.TenPhongBan
                //,nsnv.MaChucVu, nsnv.MocTinhPhep
                //                     from Sys_Users us
                //	left join NhanVien_User_Mapping map on map.User_Id = us.User_Id
                //	left join NhanVien nv on nv.NhanVien_Id = map.NhanVien_Id
                //	left join DM_PhongBan pb on pb.PhongBan_Id = nv.PhongBan_Id
                //	left join [NS_NHANVIEN] nsnv on nsnv.MaNhanVien = us.User_Code
                //                     where us.User_Code = '{request.Username}' and us.User_Password = '{hashedPassword}'
                //	";


                //         var user = await connection.QueryFirstOrDefaultAsync<UserDto>(
                //             sql, null, commandType: CommandType.Text);

                //         if (user == null)
                //             throw new UnauthorizedAccessException("Invalid username or password");

                //return _jwtService.GenerateToken(user);
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                throw;
            }
            
        }
    }
}

