using BuildingBlocks.CQRS;

namespace AuthService.Application.Users.Queries.SelectUserProfile;

public record SelectUserProfileQuery(Guid UserId) : IQuery<SelectUserProfileResponse>;
