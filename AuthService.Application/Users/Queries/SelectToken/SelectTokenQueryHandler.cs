using BuildingBlocks.CQRS;
using Common.Utils.Const;

namespace AuthService.Application.Users.Queries.SelectToken;

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