namespace ThreadService.Services;

public interface IAuthService
{
    Task<(bool, Guid)> Verify(string bearerToken);
}