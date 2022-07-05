namespace UserService.Service;

using System;
using System.Text.Json;
using System.Threading.Tasks;

using Redis.OM;
using Redis.OM.Searching;

using UserService.Model;
using UserService.Repository;

public class UserServices : IUserServices
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly RedisCollection<Users> _userCache;
    private readonly RedisConnectionProvider _provider;
    private readonly ILogger<UserServices> _logger;

    public UserServices(IUserRepository userRepository, IAuthService authService, RedisConnectionProvider provider, ILogger<UserServices> logger)
    {
        _logger = logger;
        _userRepository = userRepository;
        _authService = authService;
        _provider = provider;
        _userCache = (RedisCollection<Users>)provider.RedisCollection<Users>();
    }

    public async Task<List<Users>> GetUsers()
    {
        return await _userRepository.GetUsers();
    }

    public async Task<Users?> GetUserByEmail(string email)
    {
        var existing = await _userCache.Where(x => x.Email == email).FirstOrDefaultAsync();
        if (existing != null)
        {
            return existing;
        }
        return await _userRepository.GetUserByEmail(email);
    }

    public async Task<Users?> GetUserById(Guid id)
    {
        var existing = await _userCache.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (existing != null)
        {
            return existing;
        }
        return await _userRepository.GetUserById(id);
    }

    public async Task<IResult> NewUser(Users user)
    {
        if (user.Email == null)
        {
            _logger.LogDebug("User Didn't Provide Email. Data: {}", JsonSerializer.Serialize(user));
            return Results.BadRequest(new { Message = "Please Provide Email" });
        }
        var existing = await _userRepository.GetUserByEmail(user.Email);
        if (existing != null)
        {
            _logger.LogDebug("Found Existing Email. Data: {}", JsonSerializer.Serialize(user));
            return Results.BadRequest(new { Message = "Users Exists" });
        }
        var createdUser = await _userRepository.NewUser(user);
        if (createdUser == null)
        {
            _logger.LogDebug("Failed To Create User. Data: {}", JsonSerializer.Serialize(user));
            return Results.BadRequest(new { Message = "Failed When Create User" });
        }
        await _userCache.InsertAsync(createdUser);
        return Results.Json(new { Message = "Created", User = createdUser });
    }

    public async Task<IResult> UpdateUser(Guid id, UserUpdate user, HttpRequest request)
    {
        var verifyResult = await verifyUserAccess(request, id);
        if (verifyResult != null)
        {
            return verifyResult;
        }
        var existing = await _userRepository.GetUserById(user.Id);
        if (existing == null)
        {
            return Results.NotFound(new { Message = "User Not Found" });
        }
        existing.Name = user.Name;
        var updatedUser = await _userRepository.UpdateUser(id, existing);
        var existingCache = await _userCache.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (existingCache == null)
        {
            await _userCache.InsertAsync(updatedUser);
        }
        else
        {
            existingCache.Name = updatedUser.Name;
            await _userCache.UpdateAsync(existingCache);
        }
        return Results.Json(new { Message = "Updated", User = existing });
    }

    public async Task<IResult> UpdateUserPassword(Guid id, UserPassword user, HttpRequest request)
    {
        var verifyResult = await verifyUserAccess(request, id);
        if (verifyResult != null)
        {
            return verifyResult;
        }
        var existing = await _userRepository.GetUserById(user.Id);
        if (existing == null)
        {
            return Results.NotFound(new { Message = "User Not Found" });
        }
        existing.Password = user.Password;
        var updatedUser = await _userRepository.UpdateUserPassword(id, existing);
        if (updatedUser == null)
        {
            return Results.BadRequest(new { Message = "Failed to Update Password" });
        }
        var existingCache = await _userCache.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (existingCache == null)
        {
            await _userCache.InsertAsync(updatedUser);
        }
        else
        {
            existingCache.Password = updatedUser.Password;
            await _userCache.UpdateAsync(existingCache);
        }
        return Results.Json(new { Message = "Updated", User = updatedUser });
    }

    public async Task<IResult> DeleteUser(Guid id, HttpRequest request)
    {
        var verifyResult = await verifyUserAccess(request, id);
        if (verifyResult != null)
        {
            return verifyResult;
        }
        var existing = await _userRepository.GetUserById(id);
        if (existing == null)
        {
            return Results.NotFound(new { Message = "User Not Found" });
        }
        await _userRepository.DeleteUser(id);
        await _userCache.DeleteAsync(existing);
        return Results.Json(new { Message = "Deleted" });
    }

    private async Task<IResult?> verifyUserAccess(HttpRequest request, Guid userId)
    {
        var bearerToken = request.Headers.Authorization.ToString();
        var (success, id) = await _authService.Verify(bearerToken);
        if (!success)
        {
            return Results.Unauthorized();
        }
        if (id != userId)
        {
            return Results.Unauthorized();
        }
        return null;
    }


}