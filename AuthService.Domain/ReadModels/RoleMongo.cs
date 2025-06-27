using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthService.Domain.ReadModels;

public class RoleMongo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid RoleId { get; set; }

    public string Name { get; set; } = null!;

    public string NormalizedName { get; set; } = null!;
}