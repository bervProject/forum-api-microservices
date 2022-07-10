
namespace ThreadService.Services;

using System.Collections.Generic;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

using ThreadService.Model;
using ThreadService.Repository;

public class ThreadServices : IThreadServices
{
    private readonly IThreadRepository _threadRepository;
    private readonly IAuthService _authService;

    private readonly ILogger<ThreadServices> _logger;

    public ThreadServices(IThreadRepository threadRepository, IAuthService authService, ILogger<ThreadServices> logger)
    {
        _logger = logger;
        _threadRepository = threadRepository;
        _authService = authService;
    }

    public async Task<List<Threads>> GetThreads(Paginated paginated)
    {
        return await _threadRepository.GetThreads(paginated);
    }

    public async Task<List<Threads>> SearchThreads(string keyword)
    {
        return await _threadRepository.SearchThreads(keyword);
    }


    public async Task<Threads?> GetThreadById(Guid id)
    {
        return await _threadRepository.GetThreadById(id);
    }

    public async Task<List<Threads>> GetThreadsByUserId(Guid userId)
    {
        return await _threadRepository.GetThreadsByUserId(userId);
    }

    public async Task<IResult> Insert(Threads thread, HttpRequest request)
    {
        try
        {
            var bearerToken = request.Headers.Authorization.ToString();
            var (success, id) = await _authService.Verify(bearerToken);
            if (!success)
            {
                return Results.Unauthorized();
            }
            thread.AuthorId = id;
            var threadData = await _threadRepository.Insert(thread);
            return Results.Ok(threadData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when insert new thread.");
            return Results.BadRequest();
        }
    }

    public async Task<Threads?> Update(Guid id, Threads thread)
    {
        return await _threadRepository.Update(id, thread);
    }

    public async Task Delete(Guid id)
    {
        await _threadRepository.Delete(id);
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