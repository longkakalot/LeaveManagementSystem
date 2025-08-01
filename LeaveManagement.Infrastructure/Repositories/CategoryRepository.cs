using Dapper;
using LeaveManagement.Application.DTOs;
using LeaveManagement.Application.Interfaces;
using LeaveManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbConnectionFactory? _connectionFactory;
        private readonly IDbConnection? _connection;
        private readonly IDbTransaction? _transaction;


        // Mode 1: Query (inject vào DI cho query handler, không dùng transaction)
        public CategoryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Nhận connection và transaction từ UnitOfWork
        public CategoryRepository(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }
        public async Task<List<Countries>> GetCountries()
        {
            var sql = "sp_CATEGORIES";
            if (_connection != null)
            {
                var result = await _connection.QueryAsync<Countries>(
                sql,
                    new {  Action = "GetCountries" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var result = await conn.QueryAsync<Countries>(
                sql,
                    new { Action = "GetCountries" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public async Task<List<Provinces>> GetProvinces()
        {
            var sql = "sp_CATEGORIES";
            if (_connection != null)
            {
                var result = await _connection.QueryAsync<Provinces>(
                sql,
                    new { Action = "GetProvinces" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var result = await conn.QueryAsync<Provinces>(
                sql,
                    new { Action = "GetProvinces" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            throw new InvalidOperationException("Repository not initialized.");
        }

        public async Task<List<Wards>> GetWardsByProvinceId(int provinceId)
        {
            var sql = "sp_CATEGORIES";
            if (_connection != null)
            {
                var result = await _connection.QueryAsync<Wards>(
                sql,
                    new { ProvinceId = provinceId, Action = "GetWardsByProvinceId" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            else if (_connectionFactory != null)
            {
                using var conn = _connectionFactory.CreateQueryConnection();
                var result = await conn.QueryAsync<Wards>(
                sql,
                    new { ProvinceId = provinceId, Action = "GetWardsByProvinceId" },
                    transaction: _transaction
                );
                return result.ToList();
            }
            throw new InvalidOperationException("Repository not initialized.");
        }
    }
}
