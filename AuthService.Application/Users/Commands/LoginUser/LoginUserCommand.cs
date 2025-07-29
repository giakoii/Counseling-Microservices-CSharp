using BuildingBlocks.CQRS;

namespace AuthService.Application.Users.Commands.LoginUser;

public record LoginUserCommand(string? Email, string? Password) : ICommand<LoginResponse>;
