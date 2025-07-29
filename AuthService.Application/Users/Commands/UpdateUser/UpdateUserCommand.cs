using BuildingBlocks.CQRS;
using Common;
using Microsoft.AspNetCore.Http;

namespace AuthService.Application.Users.Commands.UpdateUser;

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
