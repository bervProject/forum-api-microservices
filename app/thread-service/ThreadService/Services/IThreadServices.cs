namespace ThreadService.Services;

using ThreadService.Model;

public interface IThreadServices
{
    Task<List<Threads>> GetThreads(Paginated paginated);
    Task<List<Threads>> SearchThreads(string keyword);
    Task<Threads?> GetThreadById(Guid id);
    Task<List<Threads>> GetThreadsByUserId(Guid userId);
    Task<Threads?> Insert(Threads thread);
    Task<Threads?> Update(Guid id, Threads thread);
    Task Delete(Guid id);
}