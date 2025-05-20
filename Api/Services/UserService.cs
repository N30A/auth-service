using System.Data;
using System.Net;
using Api.Models;
using Api.Services.Interfaces;
using Api.Services.Queries;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Api.Services;

public class UserService : IUserService
{
    private readonly IDbConnection _connection;

    public UserService(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Result<IEnumerable<UserModel>>> GetAllAsync()
    {
        try
        {
            var users = await _connection.QueryAsync<UserModel>(UserQueries.GetAll);
            return Result<IEnumerable<UserModel>>.Success("Users retrieved successfully", users);
        }
        catch (Exception)
        {   
            return Result<IEnumerable<UserModel>>.Failure();
        }
    }

    public async Task<Result<UserModel?>> GetByIdAsync(Guid userId)
    {
        try
        {   
            var user = await _connection.QuerySingleOrDefaultAsync<UserModel>(UserQueries.GetById, new { UserId = userId });
            if (user == null)
            {
                return Result<UserModel?>.NotFound("User not found");
            }
            return Result<UserModel?>.Success("User retrieved successfully", user);
        }
        catch (Exception)
        {   
            return Result<UserModel?>.Failure();
        }
    }
    
    public async Task<Result<UserModel?>> GetByEmailAsync(string email)
    {
        try
        {   
            var user = await _connection.QuerySingleOrDefaultAsync<UserModel>(UserQueries.GetByEmail, new { Email = email });
            if (user == null)
            {
                return Result<UserModel?>.NotFound("User not found");
            }
            return Result<UserModel?>.Success("User retrieved successfully", user);
        }
        catch (Exception)
        {
            return Result<UserModel?>.Failure();
        }
    }
    
    public async Task<Result<UserModel?>> AddAsync(AddUserModel newUser)
    {   
        newUser.UserId = Guid.NewGuid();
        try
        {
            var user = await _connection.QuerySingleAsync<UserModel>(UserQueries.Add, newUser);
            return Result<UserModel?>.Success("User added successfully", user, HttpStatusCode.Created);
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return Result<UserModel?>.Failure("User conflict", null, HttpStatusCode.Conflict);
        }
        catch (Exception)
        {
            return Result<UserModel?>.Failure();
        }
    }

    public async Task<Result<bool>> SoftDeleteAsync(Guid userId)
    {
        try
        {
            int rowsAffected = await _connection.ExecuteAsync(UserQueries.SoftDelete, new { UserId = userId });
            if (rowsAffected == 0)
            {   
                return Result<bool>.NotFound("User not found");
            }
            return Result<bool>.Success("User successfully deleted", true);
        }
        catch (Exception)
        {
            return Result<bool>.Failure();
        }
    }
    
    public async Task<Result<bool>> RestoreAsync(Guid userId)
    {
        try
        {
            int rowsAffected = await _connection.ExecuteAsync(UserQueries.Restore, new { UserId = userId });
            if (rowsAffected == 0)
            {
                return Result<bool>.NotFound("User not found");
            }
            return Result<bool>.Success("User successfully restored", true);
        }
        catch (Exception)
        {   
            return Result<bool>.Failure();
        }
    }
    
    public async Task<Result<UserModel?>> UpdateAsync(Guid userId, UpdateUserModel userToUpdate)
    {
        try
        {
            var user = await _connection.QuerySingleAsync<UserModel?>(UserQueries.Update, userToUpdate);
            if (user == null)
            {
                return Result<UserModel?>.NotFound("User not found");
            }
            return Result<UserModel?>.Success("User updated successfully", user);
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return Result<UserModel?>.Failure("User conflict", null, HttpStatusCode.Conflict);
        }
        catch (Exception)
        {
            return Result<UserModel?>.Failure();
        }
    }
}
