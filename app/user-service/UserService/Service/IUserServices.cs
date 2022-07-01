namespace UserService.Service;

using UserService.Model;

public interface IUserServices
{
    Task<List<Users>> GetUsers();
    Task<Users?> GetUserById(Guid id);
    Task<Users?> GetUserByEmail(string email);
    Task<Users?> NewUser(Users user);
    Task<Users?> UpdateUser(Guid id, UserUpdate user);
    Task<Users?> UpdateUserPassword(Guid id, UserPassword user);
    Task<bool> DeleteUser(Guid id);
}