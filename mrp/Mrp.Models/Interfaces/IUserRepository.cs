namespace Mrp.Models;
public interface IUserRepository {
    User? GetById(int id);
    IEnumerable<User> GetAll();
    void Add(User user);
    void Remove(int id);
}
