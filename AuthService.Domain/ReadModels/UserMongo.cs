using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthService.Domain.ReadModels;

public class UserMongo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid UserId { get; set; }

    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid RoleId { get; set; }

    public string Email { get; set; } = null!;

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? PhoneNumber { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public byte? Gender { get; set; }

    public string? Address { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public int AccessFailedCount { get; set; }

    public string? Key { get; set; }
}