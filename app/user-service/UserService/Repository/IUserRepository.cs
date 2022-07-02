namespace UserService.Repository;

using UserService.Model;

public interface IUserRepository
{
    Task<List<Users>> GetUsers();
    Task<Users?> GetUserById(Guid id);
    Task<Users?> GetUserByEmail(string email);
    Task<Users?> NewUser(Users user);
    Task<Users> UpdateUser(Guid id, Users user);
    Task<Users?> UpdateUserPassword(Guid id, Users user);
    Task DeleteUser(Guid id);
}