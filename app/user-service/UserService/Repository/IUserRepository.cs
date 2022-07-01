namespace UserService.Repository;

using UserService.Model;

public interface IUserRepository
{
    Task<List<Users>> GetUsers();
    Task<Users> GetUserById(Guid id);
    Task<Users> GetUserByEmail(string email);
    Task<Guid> NewUser(Users user);
    Task UpdateUser(Guid id, Users user);
    Task UpdateUserPassword(Guid id, Users user);
    Task DeleteUser(Guid id);
}