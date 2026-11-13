using System;
using Mrp.Models;
namespace Mrp.Data;

public interface IUserRepository {
    User? GetById(int id);
    User? GetByUsername(string username);
    List<User>? GetAll();
    void Add(User user);
    // void Remove(int id);
    void UpdateToken(string token, int id);
}