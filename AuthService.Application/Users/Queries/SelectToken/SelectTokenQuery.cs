using BuildingBlocks.CQRS;

namespace AuthService.Application.Users.Queries.SelectToken;

public record SelectTokenQuery(string RoleName) : IQuery<SelectTokenQueryResponse>;
