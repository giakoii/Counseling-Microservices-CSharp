using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;

namespace AuthService.Application.Users.Queries;

public record SelectTokenQuery(string RoleName) : IQuery<SelectTokenQueryResponse>;

public class SelectTokenQueryHandler : IQueryHandler<SelectTokenQuery, SelectTokenQueryResponse>
{
    /// <summary>
    /// Get role of the user based on the token.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<SelectTokenQueryResponse> Handle(SelectTokenQuery request, CancellationToken cancellationToken)
    {
        var response = new SelectTokenQueryResponse
        {
            Success = true,
            Response = new SelectTokenQueryEntity
            {
                Role = request.RoleName
            }
        };
        
        response.SetMessage(MessageId.I00001);
        return response;
    }
}

public class SelectTokenQueryResponse : AbstractResponse<SelectTokenQueryEntity>
{
    public override SelectTokenQueryEntity Response { get; set; }
}

public class SelectTokenQueryEntity
{
    public string Role { get; set; } = null!;
}