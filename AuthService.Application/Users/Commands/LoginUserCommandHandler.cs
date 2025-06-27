using AuthService.Domain.WriteModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Repositories;

namespace AuthService.Application.Users.Commands;

public record LoginUserCommand(string Email, string Password) : ICommand<LoginResponse>;

internal class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginResponse>
{
    private readonly ICommandRepository<User> _userRepository;
    private readonly ICommandRepository<Role> _roleRepository;

    public LoginUserCommandHandler(ICommandRepository<User> userRepository, ICommandRepository<Role> roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }


    public async Task<LoginResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var response = new LoginResponse { Success = false };
        
        // Check user exists
        var userExist = await _userRepository.Find(x => x.Email == request.Email && x.IsActive).FirstOrDefaultAsync();
        if (userExist == null)
        {
            response.Success = false;
            response.SetMessage(MessageId.E11001);
            return response;
        }

        // Check if Ecq300 updating
        if (userExist.LockoutEnd != null && userExist.Key != null)
        {
            response.Success = false;
            response.SetMessage(MessageId.E00000, "Your account has been locked");
            return response;
        }

        // Check password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, userExist.PasswordHash))
        {
            userExist.AccessFailedCount++;
            if (userExist.AccessFailedCount >= 5)
            {
                userExist.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
            }
            _userRepository.Update(userExist);
            await _userRepository.SaveChangesAsync(userExist.Email!);

            response.Success = false;
            response.SetMessage(MessageId.E00000, "The username or password is incorrect");
            return response;
        }

        // Check lockout
        if (userExist.LockoutEnd != null && userExist.LockoutEnd > DateTime.UtcNow)
        {
            response.Success = false;
            response.SetMessage(MessageId.E00000, "Your account has been locked");
            return response;
        }

        userExist.AccessFailedCount = 0;
        userExist.LockoutEnd = null;
        _userRepository.Update(userExist);
        await _userRepository.SaveChangesAsync(userExist.Email!);
        
        var role = await _roleRepository.Find(x => x.RoleId == userExist.RoleId).FirstOrDefaultAsync();

        var entityResponse = new LoginResponseEntity
        {
            UserId = userExist.UserId,
            Email = userExist.Email!,
            PhoneNumber = userExist.PhoneNumber!,
            FullName = $"{userExist.FirstName} {userExist.LastName}",
            RoleName = role!.Name,
        };
        
        // True
        response.Success = true;
        response.Response = entityResponse;
        response.SetMessage(MessageId.I00001);
        return response;
    }
}

public class LoginResponse : AbstractResponse<LoginResponseEntity>
{
    public override LoginResponseEntity Response { get; set; }
}

public class LoginResponseEntity
{
    public Guid UserId { get; set; }
    
    public string Email { get; set; } = null!;
    
    public string FullName { get; set; } = null!;
    
    public string PhoneNumber { get; set; } = null!;
    
    public string RoleName { get; set; } = null!;
}