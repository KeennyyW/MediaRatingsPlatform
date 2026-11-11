using Mrp.Models;

public class AdoNetUserRepository : IUserRepository {
    private readonly string _connectionString;
    public AdoNetUserRepository(string connectionString) {
    _connectionString = connectionString;
    }
    
    IEnumerable<User> GetAll(); 

    User? GetById(int id)
    {
        
    }
    public void Add(User user)
    {
        
    }

    public void Remove(int id)
    {
        
    
    }
}