namespace ThreadService.Services;

using ThreadService.Model;

public interface IThreadServices
{
    Task<List<Threads>> GetThreads(Paginated paginated);
    Task<List<Threads>> SearchThreads(string keyword);
    Task<Threads?> GetThreadById(Guid id);
    Task<List<Threads>> GetThreadsByUserId(Guid userId);
    Task<IResult> Insert(Threads thread, HttpRequest request);
    Task<Threads?> Update(Guid id, Threads thread);
    Task Delete(Guid id);
}