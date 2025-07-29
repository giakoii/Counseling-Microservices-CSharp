using AuthService.Domain.ReadModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Shared.Application.Interfaces;

namespace AuthService.Application.Users.Queries.SelectUserProfile;

public class SelectUserProfileQueryHandler : IQueryHandler<SelectUserProfileQuery, SelectUserProfileResponse>
{
    private readonly INoSqlQueryRepository<UserCollection> _userProfileRepository;

    public SelectUserProfileQueryHandler(INoSqlQueryRepository<UserCollection> userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<SelectUserProfileResponse> Handle(SelectUserProfileQuery request, CancellationToken cancellationToken)
    {
        var response = new SelectUserProfileResponse {Success = false};
        
        var userSelect = await _userProfileRepository.FindOneAsync(x => x.Id == request.UserId && x.IsActive);
        if (userSelect == null)
        {
            response.SetMessage(MessageId.E11001);
            return response;
        }

        var userProfileEntity = new SelectUserProfileEntity
        {
            UserId = userSelect.Id,
            Email = userSelect.Email,
            PhoneNumber = userSelect.PhoneNumber,
            FirstName = userSelect.FirstName,
            LastName = userSelect.LastName,
            DateOfBirth = userSelect.DateOfBirth,
            Address = userSelect.Address,
            AvatarUrl = userSelect.AvatarUrl,
            Gender = userSelect.Gender,
        };

        response.Response = userProfileEntity;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }
}