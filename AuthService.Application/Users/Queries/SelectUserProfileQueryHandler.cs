using AuthService.Domain.ReadModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Shared.Application.Interfaces;

namespace AuthService.Application.Users.Commands;

public record SelectUserProfileQuery(Guid UserId) : IQuery<SelectUserProfileResponse>;

internal class SelectUserProfileQueryHandler : IQueryHandler<SelectUserProfileQuery, SelectUserProfileResponse>
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