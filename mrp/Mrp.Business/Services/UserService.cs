using System;
using System.IO.Compression;
using System.Security.Cryptography;
using Mrp.Data;
using Mrp.Models;


namespace Mrp.Business;

public class UserService
{
    readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }
    public void Register(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Username and Password cant be empty");
        }    

        if (_repository.GetByUsername(username) != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);
        string stored = Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);

        var newUser = new User
        {
            Username = username,
            PasswordHash = stored
        };


        _repository.Add(newUser);
        
    }
}
