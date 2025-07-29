using Common;

namespace AuthService.Application.Users.Queries.SelectUserProfile;

public class  SelectUserProfileResponse : AbstractResponse<SelectUserProfileEntity>
{
    public override SelectUserProfileEntity Response { get; set; }
}

public class SelectUserProfileEntity
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public byte? Gender { get; set; }

    public string? Address { get; set; }

    public string? AvatarUrl { get; set; }
}