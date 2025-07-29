using System.ComponentModel.DataAnnotations;
using BuildingBlocks.CQRS;

namespace AuthService.Application.Users.Queries.SelectUser;

public record SelectUserQuery : IQuery<SelectUserResponse>
{
    [Required(ErrorMessage = "UserId is required.")]
    public Guid UserId { get; set; }
}