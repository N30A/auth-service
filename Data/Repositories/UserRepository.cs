using System.Data;
using Data.Models;
using Data.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Dapper;

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
        const string query = """
            SELECT
                UserId, Username, Email,
                CreatedAt, UpdatedAt, DeletedAt
            FROM [User];
        """;
        
        try
        {   
            _logger.LogDebug("Fetching all users from database.");
            var users = await _connection.QueryAsync<UserModel>(query);
            return DbResult<IEnumerable<UserModel>>.Success(users);
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, "Failed to fetch all users.");
            return DbResult<IEnumerable<UserModel>>.Failure();
        }
    }

    public async Task<DbResult<UserModel?>> GetByIdAsync(int userId)
    {
        const string query = """
             SELECT
                 UserId, Username, Email,
                 CreatedAt, UpdatedAt, DeletedAt
             FROM [User]
             WHERE UserId = @UserId;
         """;
        
        try
        {   
            _logger.LogDebug("Fetching user with id {UserId} from database.", userId);
            var user = await _connection.QuerySingleOrDefaultAsync<UserModel>(query, new { UserId = userId });
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
        const string query = """
             SELECT
                 UserId, Username, Email, PasswordHash,
                 CreatedAt, UpdatedAt, DeletedAt
             FROM [User]
             WHERE Email = @Email;
         """;
        
        try
        {   
            _logger.LogDebug("Fetching user with email {Email} from database.", email);
            var user = await _connection.QuerySingleOrDefaultAsync<UserModel>(query, new { Email = email });
            return DbResult<UserModel?>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch user with email {Email}.", email);
            return DbResult<UserModel?>.Failure();
        }
    }
    
    public async Task<DbResult<int?>> AddAsync(UserModel userModel)
    {
        const string query = """
             INSERT INTO [User](Username, Email, PasswordHash)
             OUTPUT inserted.UserId
             VALUES(@Username, @Email, @PasswordHash);
         """;

        try
        {
            _logger.LogDebug("Creating new user from database.");
            var status = await _connection.ExecuteScalarAsync<int?>(query, userModel);
            return DbResult<int?>.Success(status);
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, "Failed to create new user from database.");
            return DbResult<int?>.Failure();
        }
    }

    public async Task<DbResult<bool>> SoftDeleteAsync(int userId)
    {
        const string query = """
             UPDATE [User]
             SET DeletedAt = GETDATE()
             WHERE UserId = @UserId;
         """;

        try
        {   
            _logger.LogDebug("Soft-deleting user with id {UserId}.", userId);
            int rowsAffected = await _connection.ExecuteAsync(query, new { UserId = userId });
            return DbResult<bool>.Success(rowsAffected > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to soft delete user with id {UserId}.", userId);
            return DbResult<bool>.Failure();
        }
    }

    public async Task<DbResult<bool>> ChangeUsernameAsync(int userId, string newUsername)
    {
        const string query = """
             UPDATE [User]
             SET Username = @NewUsername
             WHERE UserId = @UserId;
         """;

        try
        {   
            _logger.LogDebug("Changing username for user with id {UserId} from database.", userId);
            int rowsAffected = await _connection.ExecuteAsync(query, new
            {
                UserId = userId,
                NewUsername = newUsername
            });
            return DbResult<bool>.Success(rowsAffected > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to change username for user with id {UserId}.", userId);
            return DbResult<bool>.Failure();
        }
    }

    public async Task<DbResult<bool>> ChangeEmailAsync(int userId, string newEmail)
    {
        const string query = """
             UPDATE [User]
             SET Email = @NewEmail
             WHERE UserId = @UserId;
        """;

        try
        {   
            _logger.LogDebug("Changing email for user with id {UserId} from database.", userId);
            int rowsAffected = await _connection.ExecuteAsync(query, new
            {
                UserId = userId,
                NewEmail = newEmail
            });
            return DbResult<bool>.Success(rowsAffected > 0);
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, "Failed to change email for user with id {UserId}.", userId);
            return DbResult<bool>.Failure();
        }
    }

    public async Task<DbResult<bool>> ChangePasswordAsync(int userId, string newPasswordHash)
    {
        const string query = """
             UPDATE [User]
             SET PasswordHash = @NewPasswordHash
             WHERE UserId = @UserId;
        """;

        try
        {   
            _logger.LogDebug("Changing password for user with id {UserId} from database.", userId);
            int rowsAffected = await _connection.ExecuteAsync(query, new
            {
                UserId = userId,
                NewPasswordHash = newPasswordHash
            });
            return DbResult<bool>.Success(rowsAffected > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to change password for user with id {UserId}.", userId);
            return DbResult<bool>.Failure();
        }
    }
}
