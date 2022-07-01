namespace UserService.Repository;

using UserService.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Isopoh.Cryptography.Argon2;

public class UserRepository : IUserRepository
{

    private readonly IMongoCollection<Users> _usersCollection;

    public UserRepository(IOptions<ForumApiDatabaseSettings> forumApiDatabaseSettings)
    {
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
            return null;
        }
        var hashPassword = Argon2.Hash(user.Password);
        user.Password = hashPassword;
        await _usersCollection.InsertOneAsync(user);
        return user;
    }

    public async Task<Users> UpdateUser(Guid id, Users user)
    {
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