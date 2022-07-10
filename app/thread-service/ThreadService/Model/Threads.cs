namespace ThreadService.Model;

using System.ComponentModel.DataAnnotations;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

using Redis.OM.Modeling;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "Threads" }, IndexName = "threads-idx")]
public class Threads
{
    [BsonId(IdGenerator = typeof(CombGuidGenerator))]
    [RedisIdField]
    public Guid Id { get; set; }
    [Required]
    [Indexed]
    public string? Title { get; set; }
    [Required]
    [Indexed]
    public string? Post { get; set; }
    [Indexed]
    public string? Category { get; set; }
    [Indexed]
    public List<string>? Tags { get; set; }
    [Required]
    [Indexed]
    public Guid AuthorId { get; set; }
    [BsonIgnore]
    public Users? User { get; set; }
}