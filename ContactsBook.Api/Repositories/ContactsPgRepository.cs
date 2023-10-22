using System.Data;
using Dapper;
using Npgsql;
using ContactsBook.Api.Repositories;

namespace ContactsBook.Api.Repositories;

public interface IContactsPgRepository
{
    Task<IEnumerable<Contact>> GetAllAsync();
    Task<Contact> GetByIdAsync(string id);
    Task CreateAsync(Contact contact);
    Task UpdateAsync(Contact contact);
    Task DeleteAsync(string id);
}

/// <summary>
/// Create the db and table
/// CREATE TABLE IF NOT EXISTS contacts (Id VARCHAR, FirstName VARCHAR, LastName VARCHAR, Email VARCHAR);
/// </summary>
public class ContactsPgRepository : IContactsPgRepository
{
    private string _connectionString;

    public ContactsPgRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    public async Task<IEnumerable<Contact>> GetAllAsync()
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM contacts";
        return await connection.QueryAsync<Contact>(sql);
    }

    public async Task<Contact> GetByIdAsync(string id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM contacts " +
                "WHERE Key = @id";
        return await connection.QuerySingleOrDefaultAsync<Contact>(sql, new { id });
    }

    public async Task CreateAsync(Contact contact)
    {
        using var connection = CreateConnection();
        var sql = "INSERT INTO Contacts (Key, FirstName, LastName, Email) " +
                "VALUES (@Key, @FirstName, @LastName, @Email)";
        await connection.ExecuteAsync(sql, contact);
    }

    public async Task UpdateAsync(Contact contact)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Contacts " +
                "SET FirstName = @FirstName, " +
                "    LastName = @LastName, " +
                "    Email = @Email " +
                "WHERE Key = @Key";
        await connection.ExecuteAsync(sql, contact);
    }

    public async Task DeleteAsync(string id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Contacts " +
                "WHERE Key IN (@id)";
        await connection.ExecuteAsync(sql, new { id });
    }
}