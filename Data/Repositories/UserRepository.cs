using Data.Models;
using Data.Repositories.Interfaces;
using System.Data;
using Dapper;

namespace Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _connection;

    public UserRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        const string query = """
            SELECT
                UserId, Username, Email, PasswordHash,
                CreatedAt, UpdatedAt, DeletedAt,
            FROM [User]
            WHERE DeletedAt IS NULL;
        """;
        
        try
        {
            return await _connection.QueryAsync<User>(query);
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        const string query = """
             SELECT
                 UserId, Username, Email, PasswordHash,
                 CreatedAt, UpdatedAt, DeletedAt,
             FROM [User]
             WHERE DeletedAt IS NULL AND UserId = @UserId;
         """;
        
        try
        {
            return await _connection.QuerySingleOrDefaultAsync(query, new { UserId = userId });
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        const string query = """
             SELECT
                 UserId, Username, Email, PasswordHash,
                 CreatedAt, UpdatedAt, DeletedAt,
             FROM [User]
             WHERE DeletedAt IS NULL AND Email = @Email;
         """;
        
        try
        {
            return await _connection.QuerySingleOrDefaultAsync(query, new { Email = email });
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public async Task<int?> AddAsync(User user)
    {
        const string query = """
             INSERT INTO [User](Username, Email, PasswordHash)
             OUTPUT inserted.UserId
             VALUES(@Username, @Email, @PasswordHash);
         """;

        try
        {
            return await _connection.QuerySingleOrDefaultAsync<int?>(query, user);
        }
        catch (Exception)
        {
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
            int rowsAffected = await _connection.ExecuteAsync(query, new { UserId = userId });
            return rowsAffected >= 1;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ChangeUsernameAsync(int userId, string newUsername)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ChangeEmailAsync(int userId, string newEmail)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ChangePasswordAsync(int userId, string newPasswordHash)
    {
        throw new NotImplementedException();
    }
}
