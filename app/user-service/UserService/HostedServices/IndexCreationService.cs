namespace UserService.HostedServices;

using Microsoft.Extensions.Logging;

using Redis.OM;

using UserService.Model;

public class IndexCreationService : IHostedService
{
    private readonly RedisConnectionProvider _provider;
    private readonly ILogger<IndexCreationService> _logger;

    public IndexCreationService(RedisConnectionProvider provider, ILogger<IndexCreationService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Create Index {}", typeof(Users));
        var result = await _provider.Connection.CreateIndexAsync(typeof(Users));
        _logger.LogDebug("Create Index {} Result: {}", typeof(Users), result);

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}