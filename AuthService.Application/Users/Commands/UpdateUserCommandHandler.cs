using AuthService.Application.Services;
using AuthService.Domain.ReadModels;
using AuthService.Domain.WriteModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Interfaces;

namespace AuthService.Application.Users.Commands;

public record UpdateUserCommand : ICommand<BaseCommandResponse>
{
    public string? Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }
    
    public string? FirstName { get; set; } = null!;
    
    public string? LastName { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public byte? Gender { get; set; }

    public string? Address { get; set; }

    public IFormFile? AvatarFile { get; set; }
}

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, BaseCommandResponse>
{
    private readonly ICommandRepository<User> _userRepository;
    private readonly INoSqlQueryRepository<UserCollection> _userCollectionRepository;
    private readonly IIdentityService _identityService;
    private readonly IUploadImageService _uploadImageService;

    public UpdateUserCommandHandler(ICommandRepository<User> userRepository, IIdentityService identityService, INoSqlQueryRepository<UserCollection> userCollectionRepository, IUploadImageService uploadImageService)
    {
        _userRepository = userRepository;
        _identityService = identityService;
        _userCollectionRepository = userCollectionRepository;
        _uploadImageService = uploadImageService;
    }

    public async Task<BaseCommandResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseCommandResponse { Success = false };

        // Get the current user
        var currentUser = _identityService.GetCurrentUser();

        // Find the user in the repository
        var user = await _userRepository.Find(x => x.Id == Guid.Parse(currentUser.UserId) && x.IsActive).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        var userCollection = await _userCollectionRepository.FindOneAsync(x => x.Id == Guid.Parse(currentUser.UserId) && x.IsActive);
        if (user == null || userCollection == null)
        {
            response.SetMessage(MessageId.E11001);
            return response;
        }

        await _userRepository.ExecuteInTransactionAsync(async () =>
        {
            // Update user properties
            user.Email = request.Email ?? user.Email;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.DateOfBirth = request.DateOfBirth ?? user.DateOfBirth;
            user.Gender = request.Gender ?? user.Gender;
            user.Address = request.Address ?? user.Address;
            if (request.AvatarFile != null)
            {
                user.AvatarUrl = await _uploadImageService.UploadImageAsync(request.AvatarFile);
            }
            
            // Update user collection properties
            userCollection.Email = user.Email;
            userCollection.PhoneNumber = user.PhoneNumber;
            userCollection.FirstName = user.FirstName;
            userCollection.LastName = user.LastName;
            userCollection.DateOfBirth = user.DateOfBirth;
            userCollection.AvatarUrl = user.AvatarUrl;
            userCollection.Address = user.Address;
            userCollection.Gender = user.Gender;
            
            // Save changes
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(currentUser.Email);
            
            _userRepository.Store(userCollection, currentUser.Email, true);
            await _userRepository.SessionSavechanges();
            
            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
}