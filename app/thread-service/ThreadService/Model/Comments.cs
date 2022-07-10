namespace ThreadService.Model;

using System.ComponentModel.DataAnnotations;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

using Redis.OM.Modeling;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "Comments" }, IndexName = "comments-idx")]
public class Comments
{
    [BsonId(IdGenerator = typeof(CombGuidGenerator))]
    [RedisIdField]
    public Guid Id { get; set; }
    [Required]
    [Indexed]
    public string? Comment { get; set; }
    [Required]
    [Indexed]
    public Guid ThreadId { get; set; }
    [Indexed]
    public Guid? ReplyId { get; set; }
    [Required]
    [Indexed]
    public Guid AuthorId { get; set; }
}