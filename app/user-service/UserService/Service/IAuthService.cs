namespace UserService.Service;

public interface IAuthService
{
    Task<(bool, Guid)> Verify(string bearerToken);
}