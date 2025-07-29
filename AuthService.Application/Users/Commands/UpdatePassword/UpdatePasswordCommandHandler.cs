using AuthService.Domain.WriteModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Shared.Application.Interfaces;

namespace AuthService.Application.Users.Commands.UpdatePassword;

/// <summary>
/// UpdatePasswordCommandHandler - Handles the command to update a user's password.
/// </summary>
public class UpdatePasswordCommandHandler : ICommandHandler<UpdatePasswordCommand, BaseCommandResponse>
{
    private readonly ICommandRepository<User> _userRepository;
    private readonly IIdentityService _identityService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userRepository"></param>
    /// <param name="identityService"></param>
    public UpdatePasswordCommandHandler(ICommandRepository<User> userRepository, IIdentityService identityService)
    {
        _userRepository = userRepository;
        _identityService = identityService;
    }

    public async Task<BaseCommandResponse> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseCommandResponse { Success = false };

        // Get the current user
        var currentUser = _identityService.GetCurrentUser();

        // Find the user in the repository
        var user = _userRepository.Find(x => x.Id == Guid.Parse(currentUser.UserId) && x.IsActive).FirstOrDefault();
        if (user == null)
        {
            response.SetMessage(MessageId.E11001);
            return response;
        }

        // Validate old password
        if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
        {
            response.SetMessage(MessageId.E00000, "Old password is incorrect.");
            return response;
        }

        // Update the password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);
        
        // Save changes to the repository
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(currentUser.Email);
        
        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }
}