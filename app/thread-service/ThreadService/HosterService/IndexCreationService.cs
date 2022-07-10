namespace ThreadService.HostedServices;

using System.Text.Json;


using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MongoDB.Driver;

using Newtonsoft.Json;


using Redis.OM;

using ThreadService.Model;

public class IndexCreationService : IHostedService
{
    private readonly IMongoCollection<Threads> _threadsCollection;
    private readonly ILogger<IndexCreationService> _logger;

    public IndexCreationService(IOptions<ForumApiDatabaseSettings> forumApiDatabaseSettings, ILogger<IndexCreationService> logger)
    {
        _logger = logger;
        var mongoClient = new MongoClient(forumApiDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(forumApiDatabaseSettings.Value.DatabaseName);
        _threadsCollection = mongoDatabase.GetCollection<Threads>(forumApiDatabaseSettings.Value.ThreadsCollectionName);

    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Create Index {}", typeof(Threads));
        var indexDefine = Builders<Threads>.IndexKeys.Text(indexKey => indexKey.Title).Text(indexKey => indexKey.Post);
        var result = await _threadsCollection.Indexes.CreateOneAsync(new CreateIndexModel<Threads>(indexDefine));
        _logger.LogDebug("Create Index for: {}. Result: {}", typeof(Threads), result);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}