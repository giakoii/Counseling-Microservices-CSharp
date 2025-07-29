using System.ComponentModel.DataAnnotations;
using BuildingBlocks.CQRS;
using Common;

namespace AuthService.Application.Users.Commands.UpdatePassword;

public record UpdatePasswordCommand : ICommand<BaseCommandResponse>
{
    [Required(ErrorMessage = "Old password is required")]
    public string OldPassword { get; set; } = null!;
    
    [Required(ErrorMessage = "New password is required")]
    public string NewPassword { get; set; } = null!;
}