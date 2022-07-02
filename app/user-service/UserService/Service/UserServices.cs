namespace UserService.Service;

using System;
using System.Threading.Tasks;

using Redis.OM;
using Redis.OM.Searching;

using UserService.Model;
using UserService.Repository;

public class UserServices : IUserServices
{
    private readonly IUserRepository _userRepository;
    private readonly RedisCollection<Users> _userCache;
    private readonly RedisConnectionProvider _provider;

    public UserServices(IUserRepository userRepository, RedisConnectionProvider provider)
    {
        _userRepository = userRepository;
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

    public async Task<Users?> NewUser(Users user)
    {
        if (user.Email == null)
        {
            return null;
        }
        var existing = await _userRepository.GetUserByEmail(user.Email);
        if (existing != null)
        {
            return null;
        }
        var createdUser = await _userRepository.NewUser(user);
        if (createdUser == null)
        {
            return null;
        }
        await _userCache.InsertAsync(createdUser);
        return createdUser;
    }

    public async Task<Users?> UpdateUser(Guid id, UserUpdate user)
    {
        var existing = await _userRepository.GetUserById(user.Id);
        if (existing == null)
        {
            return null;
        }
        existing.Name = user.Name;
        var updatedUser = await _userRepository.UpdateUser(id, existing);
        var exisingCache = await _userCache.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (exisingCache == null)
        {
            await _userCache.InsertAsync(updatedUser);
        }
        else
        {
            exisingCache.Name = updatedUser.Name;
            await _userCache.Update(exisingCache);
        }
        return updatedUser;
    }

    public async Task<Users?> UpdateUserPassword(Guid id, UserPassword user)
    {
        var existing = await _userRepository.GetUserById(user.Id);
        if (existing == null)
        {
            return null;
        }
        existing.Password = user.Password;
        var updatedUser = await _userRepository.UpdateUserPassword(id, existing);
        if (updatedUser == null)
        {
            return null;
        }
        var exisingCache = await _userCache.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (exisingCache == null)
        {
            await _userCache.InsertAsync(updatedUser);
        }
        else
        {
            exisingCache.Password = updatedUser.Password;
            await _userCache.Update(exisingCache);
        }
        return updatedUser;
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        var existing = await _userRepository.GetUserById(id);
        if (existing == null)
        {
            return false;
        }
        await _userRepository.DeleteUser(id);
        await _userCache.Delete(existing);
        return true;
    }


}