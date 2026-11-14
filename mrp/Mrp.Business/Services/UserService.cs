using System;
using System.IO.Compression;
using System.Security.Authentication;
using System.Security.Cryptography;
using Mrp.Data;
using Mrp.Models;
using Superpower.Model;


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

    public string Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Username and Password cant be empty");
        }    

        User? user = _repository.GetByUsername(username);

        if (user != null)
        {

            string[] parts = user.PasswordHash.Split(':');
            byte[] salt = Convert.FromBase64String(parts[0]);

            byte[] expectedHash = Convert.FromBase64String(parts[1]);
            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);

            if (!CryptographicOperations.FixedTimeEquals(actualHash, expectedHash))
            {
                throw new InvalidCredentialException("Password does not match");
            }
            else
            {
                string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                user.Token = token;
                _repository.UpdateToken(token, user.Id);

                return token;
            } 

        }
        else
        {
            throw new InvalidCredentialException("Password does not match");
        }
        
    }


    public User? GetUserByToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        return _repository.GetByToken(token);
    }
}
