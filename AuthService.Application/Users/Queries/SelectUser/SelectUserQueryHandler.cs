using AuthService.Application.Users.Queries.SelectUserProfile;
using AuthService.Domain.ReadModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Shared.Application.Interfaces;

namespace AuthService.Application.Users.Queries.SelectUser;


/// <summary>
/// SelectUserQueryHandler - Handles the selection of a user profile by UserId.
/// </summary>
public class SelectUserQueryHandler : IQueryHandler<SelectUserQuery, SelectUserResponse>
{
    private readonly INoSqlQueryRepository<UserCollection> _userRepository;

    public SelectUserQueryHandler(INoSqlQueryRepository<UserCollection> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<SelectUserResponse> Handle(SelectUserQuery request, CancellationToken cancellationToken)
    {
        var response = new SelectUserResponse { Success = false };

        var user = await _userRepository.FindOneAsync(x => x.Id == request.UserId && x.IsActive);
        if (user == null)
        {
            response.SetMessage(MessageId.E11001);
            return response;
        }

        response.Response = new SelectUserProfileEntity
        {
            UserId = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            AvatarUrl = user.AvatarUrl,
            Gender = user.Gender,
        };

        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }
}

public class SelectUserResponse : AbstractResponse<SelectUserProfileEntity>
{
    public override SelectUserProfileEntity Response { get; set; }
}