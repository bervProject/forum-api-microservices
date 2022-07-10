
namespace ThreadService.Repository;

using System.Collections.Generic;

using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

using MongoDB.Bson;
using MongoDB.Driver;

using ThreadService.Model;


public class ThreadRepository : IThreadRepository
{
    private readonly IMongoCollection<Users> _usersCollection;
    private readonly IMongoCollection<Threads> _threadsCollection;
    private readonly ILogger<ThreadRepository> _logger;

    public ThreadRepository(IOptions<ForumApiDatabaseSettings> forumApiDatabaseSettings, ILogger<ThreadRepository> logger)
    {
        _logger = logger;
        var mongoClient = new MongoClient(forumApiDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(forumApiDatabaseSettings.Value.DatabaseName);
        _usersCollection = mongoDatabase.GetCollection<Users>(forumApiDatabaseSettings.Value.UsersCollectionName);
        _threadsCollection = mongoDatabase.GetCollection<Threads>(forumApiDatabaseSettings.Value.ThreadsCollectionName);
    }

    public async Task<List<Threads>> GetThreads(Paginated paginated)
    {
        var skip = paginated.Page * paginated.Limit;
        var threads = await _threadsCollection.Find(_ => true).Skip(skip).Limit(paginated.Limit).ToListAsync();
        return await populateThreads(threads);
    }

    public async Task<List<Threads>> SearchThreads(string keyword)
    {
        var searchFilter = Builders<Threads>.Filter.Text(keyword, new TextSearchOptions
        {
            CaseSensitive = false,
        });
        var threads = await _threadsCollection.Find(searchFilter).ToListAsync();
        return await populateThreads(threads);
    }


    public async Task<Threads?> GetThreadById(Guid id)
    {
        var thread = await _threadsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return await populateThread(thread);
    }

    public async Task<List<Threads>> GetThreadsByUserId(Guid userId)
    {
        var threads = await _threadsCollection.Find(x => x.AuthorId == userId).ToListAsync();
        return await populateThreads(threads);
    }

    public async Task<Threads?> Insert(Threads thread)
    {
        try
        {
            await _threadsCollection.InsertOneAsync(thread);
            return thread;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when insert new thread.");
            return null;
        }
    }

    public async Task<Threads?> Update(Guid id, Threads thread)
    {
        await _threadsCollection.ReplaceOneAsync(x => x.Id == id, thread, new ReplaceOptions
        {
            IsUpsert = false,
        });
        return thread;
    }

    public async Task Delete(Guid id)
    {
        await _threadsCollection.DeleteOneAsync(x => x.Id == id);
    }

    private async Task<List<Threads>> populateThreads(List<Threads> threads)
    {
        var userIds = threads.Select(x => x.AuthorId).Distinct();
        var filter = Builders<Users>.Filter.In(x => x.Id, userIds);
        var users = await _usersCollection.Find(filter).ToListAsync();
        _logger.LogDebug(string.Join(",", users));
        return threads.Select(x =>
        {
            x.User = users.Find(user => user.Id == x.AuthorId);
            return x;
        }).ToList();
    }

    private async Task<Threads?> populateThread(Threads? thread)
    {
        if (thread == null)
        {
            return null;
        }
        var user = await _usersCollection.Find(x => thread.AuthorId == x.Id).FirstAsync();
        thread.User = user;
        return thread;
    }
}