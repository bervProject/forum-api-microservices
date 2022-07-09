namespace ThreadService.Model;

public class ForumApiDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string UsersCollectionName { get; set; } = null!;
    public string ThreadsCollectionName { get; set; } = null!;
    public string CommentsCollectionName { get; set; } = null!;
}
