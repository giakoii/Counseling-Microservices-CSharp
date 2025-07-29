using BuildingBlocks.CQRS;
using Common;

namespace AuthService.Application.Users.Commands.InsertCounselor;

public record InsertCounselorCommand(
    string Email,
    string FirstName,
    string LastName
) : ICommand<BaseCommandResponse>;
