namespace ThreadService.Model;

using System.ComponentModel.DataAnnotations;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

using Redis.OM.Modeling;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "ThreadUpvotes" }, IndexName = "thread-upvotes-idx")]
public class ThreadUpvotes
{
    [BsonId(IdGenerator = typeof(CombGuidGenerator))]
    [RedisIdField]
    public Guid Id { get; set; }
    [Required]
    [Indexed]
    public Guid ThreadId { get; set; }
    [Required]
    [Indexed]
    public Guid AuthorId { get; set; }
}