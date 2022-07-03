namespace UserService.Repository;

using System.Text.Json;

using Isopoh.Cryptography.Argon2;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

using UserService.Model;

public class UserRepository : IUserRepository
{

    private readonly IMongoCollection<Users> _usersCollection;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IOptions<ForumApiDatabaseSettings> forumApiDatabaseSettings, ILogger<UserRepository> logger)
    {
        _logger = logger;
        var mongoClient = new MongoClient(forumApiDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(forumApiDatabaseSettings.Value.DatabaseName);
        _usersCollection = mongoDatabase.GetCollection<Users>(forumApiDatabaseSettings.Value.UsersCollectionName);
    }

    public async Task<List<Users>> GetUsers()
    {
        return await _usersCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Users?> GetUserById(Guid id)
    {
        return await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Users?> GetUserByEmail(string email)
    {
        return await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
    }

    public async Task<Users?> NewUser(Users user)
    {
        if (user.Password == null)
        {
            _logger.LogDebug("Didn't provide user Password when Create User. Data: {}", JsonSerializer.Serialize(user));
            return null;
        }
        var hashPassword = Argon2.Hash(user.Password);
        user.Password = hashPassword;
        await _usersCollection.InsertOneAsync(user);
        return user;
    }

    public async Task<Users> UpdateUser(Guid id, Users user)
    {
        user.Id = id;
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, user, new ReplaceOptions()
        {
            IsUpsert = false,
        });
        return user;
    }

    public async Task<Users?> UpdateUserPassword(Guid id, Users user)
    {
        if (user.Password == null)
        {
            return null;
        }
        user.Id = id;
        var hashPassword = Argon2.Hash(user.Password);
        user.Password = hashPassword;
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, user, new ReplaceOptions()
        {
            IsUpsert = false,
        });
        return user;
    }

    public async Task DeleteUser(Guid id)
    {
        await _usersCollection.DeleteOneAsync(x => x.Id == id);
    }
}