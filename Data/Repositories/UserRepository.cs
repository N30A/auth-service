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

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        const string query = """
            SELECT
                UserId, Username, Email, PasswordHash,
                CreatedAt, UpdatedAt, DeletedAt
            FROM [User]
        """;
        
        try
        {   
            _logger.LogDebug("Fetching all users from database.");
            return await _connection.QueryAsync<UserDto>(query);
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, "Failed to fetch all users.");
            return [];
        }
    }

    public async Task<UserDto?> GetByIdAsync(int userId)
    {
        const string query = """
             SELECT
                 UserId, Username, Email, PasswordHash,
                 CreatedAt, UpdatedAt, DeletedAt
             FROM [User]
             WHERE UserId = @UserId;
         """;
        
        try
        {   
            _logger.LogDebug("Fetching user with id {UserId} from database.", userId);
            return await _connection.QuerySingleOrDefaultAsync(query, new { UserId = userId });
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, "Failed to fetch user with id {UserId}.", userId);
            return null;
        }
    }
    
    public async Task<UserDto?> GetByEmailAsync(string email)
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
            return await _connection.QuerySingleOrDefaultAsync(query, new { Email = email });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch user with email {Email}.", email);
            return null;
        }
    }
    
    public async Task<int?> AddAsync(UserDto userDto)
    {
        const string query = """
             INSERT INTO [User](Username, Email, PasswordHash)
             OUTPUT inserted.UserId
             VALUES(@Username, @Email, @PasswordHash);
         """;

        try
        {
            _logger.LogDebug("Creating new user from database.");
            return await _connection.ExecuteScalarAsync<int?>(query, userDto);
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, "Failed to create new user from database.");
            return null;
        }
    }

    public async Task<bool> SoftDeleteAsync(int userId)
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
            return rowsAffected >= 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to soft delete user with id {UserId}.", userId);
            return false;
        }
    }

    public async Task<bool> ChangeUsernameAsync(int userId, string newUsername)
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
            return rowsAffected >= 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to change username for user with id {UserId}.", userId);
            return false;
        }
    }

    public async Task<bool> ChangeEmailAsync(int userId, string newEmail)
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
            return rowsAffected >= 1;
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, "Failed to change email for user with id {UserId}.", userId);
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(int userId, string newPasswordHash)
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
            return rowsAffected >= 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to change password for user with id {UserId}.", userId);
            return false;
        }
    }
}
