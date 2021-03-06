namespace ThreadService.Model;

using System.ComponentModel.DataAnnotations;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

using Redis.OM.Modeling;

[BsonIgnoreExtraElements]
[Document(StorageType = StorageType.Json, Prefixes = new[] { "Users" }, IndexName = "users-idx")]
public class Users
{
    [BsonId(IdGenerator = typeof(CombGuidGenerator))]
    [RedisIdField]
    public Guid Id { get; set; }
    [Required]
    [Indexed]
    public string? Name { get; set; }
    [Required]
    [Indexed]
    public string? Email { get; set; }
}