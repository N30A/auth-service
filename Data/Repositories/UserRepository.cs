using System.Data;
using Data.Models;
using Data.Repositories.Interfaces;
using Data.Repositories.Queries;
using Microsoft.Extensions.Logging;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _connection;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IDbConnection connection, ILogger<UserRepository> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task<DbResult<IEnumerable<UserModel>>> GetAllAsync()
    {
        try
        {
            _logger.LogDebug("Fetching users from database.");
            var users = await _connection.QueryAsync<UserModel>(UserQueries.GetAll);
            _logger.LogDebug("Successfully fetched users from database.");
            return DbResult<IEnumerable<UserModel>>.Success(users);
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, "Unexpected error occured while fetching users from database.");
            return DbResult<IEnumerable<UserModel>>.Failure();
        }
    }

    public async Task<DbResult<UserModel?>> GetByIdAsync(int userId)
    {
        try
        {   
            _logger.LogDebug("Fetching user with id {UserId} from database.", userId);
            var user = await _connection.QuerySingleOrDefaultAsync<UserModel>(UserQueries.GetById, new { UserId = userId });
            if (user == null)
            {
                _logger.LogDebug("User with id {UserId} not found.", userId);
                return DbResult<UserModel?>.Failure(user, DbErrorType.NotFound);
            }
            
            _logger.LogDebug("Successfully fetched user with id {UserId} from database.", userId);
            return DbResult<UserModel?>.Success(user);
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, "Failed to fetch user with id {UserId}.", userId);
            return DbResult<UserModel?>.Failure();
        }
    }
    
    public async Task<DbResult<UserModel?>> GetByEmailAsync(string email)
    {
        try
        {   
            _logger.LogDebug("Fetching user with email {Email} from database.", email);
            var user = await _connection.QuerySingleOrDefaultAsync<UserModel>(UserQueries.GetByEmail, new { Email = email });
            if (user == null)
            {
                _logger.LogDebug("User with email {Email} not found.", email);
                return DbResult<UserModel?>.Failure(user, DbErrorType.NotFound);
            }
            
            _logger.LogDebug("Successfully fetched user with email {Email} from database.", email);
            return DbResult<UserModel?>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch user with email {Email}.", email);
            return DbResult<UserModel?>.Failure();
        }
    }
    
    public async Task<DbResult<UserModel?>> AddAsync(AddUserModel newUser)
    {
        try
        {
            _logger.LogDebug("Adding new user with email {Email} to database", newUser.Email);
            var user = await _connection.QuerySingleAsync<UserModel>(UserQueries.Add, newUser);
            
            _logger.LogDebug("Successfully added user with email {Email} to database", newUser.Email);
            return DbResult<UserModel?>.Success(user);
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            _logger.LogError(ex, "Failed to add new user with email {Email} due to constraint.", newUser.Email);
            return DbResult<UserModel?>.Failure(null, DbErrorType.UniqueConstraintViolation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occured while adding user with email {Email} to database", newUser.Email);
            return DbResult<UserModel?>.Failure();
        }
    }

    public async Task<DbResult<bool>> SoftDeleteAsync(int userId)
    {
        try
        {   
            _logger.LogDebug("Soft-deleting user with id {UserId}.", userId);
            int rowsAffected = await _connection.ExecuteAsync(UserQueries.SoftDelete, new { UserId = userId });
            
            if (rowsAffected == 0)
            {   
                _logger.LogDebug("User with id {UserId} not found for soft-delete.", userId);
                return DbResult<bool>.Failure(false, DbErrorType.NotFound);
            }
        
            _logger.LogDebug("Successfully soft-deleted user with id {UserId}.", userId);
            return DbResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while soft-deleting user with id {UserId}.", userId);
            return DbResult<bool>.Failure();
        }
    }
    
    public async Task<DbResult<bool>> RestoreAsync(int userId)
    {
        try
        {   
            _logger.LogDebug("Restoring user with id {UserId} in database.", userId);
            int rowsAffected = await _connection.ExecuteAsync(UserQueries.Restore, new { UserId = userId });
            
            if (rowsAffected == 0)
            {
                _logger.LogDebug("User with id {UserId} not found for restoring.", userId);
                return DbResult<bool>.Failure(false, DbErrorType.NotFound);
            }
            
            _logger.LogDebug("Successfully restored user with id {UserId}.", userId);
            return DbResult<bool>.Success(true);
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, "Unexpected error occurred while restoring user with id {UserId}.", userId);
            return DbResult<bool>.Failure();
        }
    }
    
    public async Task<DbResult<bool>> ChangeUsernameAsync(int userId, string newUsername)
    {
        try
        {   
            _logger.LogDebug("Changing username for user with id {UserId} from database.", userId);
            int rowsAffected = await _connection.ExecuteAsync(UserQueries.ChangeUsername, new
            {
                UserId = userId,
                NewUsername = newUsername
            });

            if (rowsAffected == 0)
            {
                _logger.LogDebug("User with id {UserId} not found for changing username.", userId);
                return DbResult<bool>.Failure(false, DbErrorType.NotFound);
            }
            
            _logger.LogDebug("Successfully changed username for user with id {UserId}.", userId);
            return DbResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while changing username for user with id {UserId}.", userId);
            return DbResult<bool>.Failure();
        }
    }
    
    public async Task<DbResult<bool>> ChangeEmailAsync(int userId, string newEmail)
    {
        try
        {   
            _logger.LogDebug("Changing email for user with id {UserId} from database.", userId);
            int rowsAffected = await _connection.ExecuteAsync(UserQueries.ChangeEmail, new
            {
                UserId = userId,
                NewEmail = newEmail
            });
            
            if (rowsAffected == 0)
            {
                _logger.LogDebug("User with id {UserId} not found for changing email.", userId);
                return DbResult<bool>.Failure(false, DbErrorType.NotFound);
            }
            
            _logger.LogDebug("Successfully changed email for user with id {UserId}.", userId);
            return DbResult<bool>.Success(true);
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, "Unexpected error occurred while changing email for user with id {UserId}.", userId);
            return DbResult<bool>.Failure();
        }
    }

    public async Task<DbResult<bool>> ChangePasswordAsync(int userId, string newPasswordHash)
    {
        try
        {   
            _logger.LogDebug("Changing password for user with id {UserId} from database.", userId);
            int rowsAffected = await _connection.ExecuteAsync(UserQueries.ChangePassword, new
            {
                UserId = userId,
                NewPasswordHash = newPasswordHash
            });
            
            if (rowsAffected == 0)
            {
                _logger.LogDebug("User with id {UserId} not found for changing password.", userId);
                return DbResult<bool>.Failure(false, DbErrorType.NotFound);
            }
            
            _logger.LogDebug("Successfully changed password for user with id {UserId}.", userId);
            return DbResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while changing password for user with id {UserId}.", userId);
            return DbResult<bool>.Failure();
        }
    }
}
