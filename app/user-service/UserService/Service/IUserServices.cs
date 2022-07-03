namespace UserService.Service;

using UserService.Model;

public interface IUserServices
{
    Task<List<Users>> GetUsers();
    Task<Users?> GetUserById(Guid id);
    Task<Users?> GetUserByEmail(string email);
    Task<IResult> NewUser(Users user);
    Task<IResult> UpdateUser(Guid id, UserUpdate user, HttpRequest request);
    Task<IResult> UpdateUserPassword(Guid id, UserPassword user, HttpRequest request);
    Task<IResult> DeleteUser(Guid id, HttpRequest request);
}