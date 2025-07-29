using BuildingBlocks.CQRS;
using Common;

namespace AuthService.Application.Users.Commands.InsertUser;

public record InsertUserCommand (
    string Email,
    string Password,
    string FirstName,
    string LastName
) : ICommand<BaseCommandResponse>;