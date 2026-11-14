using Mrp.Models;

namespace Mrp.Data;

using System.Configuration.Assemblies;
using Microsoft.VisualBasic;
using Npgsql;


public class AdoNetUserRepository : IUserRepository {
    private readonly string ConnectionString;
    
    public AdoNetUserRepository(string connectionString) {
    ConnectionString = connectionString;
    }
    
    public List<User>? GetAll()
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand("SELECT * from users", connection);

        var users = new List<User>();

        using var reader = cmd.ExecuteReader();


        
        while (reader.Read())
        {
            var user = new User
            {
              Id = reader.GetInt32(0),
              Username = reader.GetString(1),
              PasswordHash = reader.GetString(2),
              CreatedAt = reader.GetDateTime(3)
            };
            users.Add(user);
        }
        return users;
    }

    public User? GetById(int id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand("SELECT * FROM users WHERE id = @id", connection);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        var user = new User
        {
            Id = id,
            Username = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            CreatedAt = reader.GetDateTime(3)
        };

        return user;
    }   

    public User? GetByUsername(string username)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", connection);
        cmd.Parameters.AddWithValue("@username", username);

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }
        
        var user = new User
        {
            Id = reader.GetInt32(0),
            Username = username,
            PasswordHash = reader.GetString(2),
            CreatedAt = reader.GetDateTime(3)
        };

        return user;

    }
    public User? GetByToken(string token)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM users WHERE token = @token", connection);
        cmd.Parameters.AddWithValue("@token", token);

        using var reader = cmd.ExecuteReader();


        if (!reader.Read())
        {
            return null;
        }
        
        var user = new User
        {
            Id = reader.GetInt32(0),
            Username = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            CreatedAt = reader.GetDateTime(3),
            Token = reader.GetString(4)
        };

        return user;
    }

    public void Add(User user)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand("INSERT INTO users (username, passwordhash) VALUES (@username, @passwordhash)", connection);
        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@passwordhash", user.PasswordHash);
        cmd.ExecuteNonQuery();
    }

    public void UpdateToken(string token, int id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();


        using var cmd = new NpgsqlCommand("UPDATE users SET token = @token WHERE id = @id", connection);
        cmd.Parameters.AddWithValue("@token", token);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    // public void Remove(int id)
    // {
        
    
    // }
}