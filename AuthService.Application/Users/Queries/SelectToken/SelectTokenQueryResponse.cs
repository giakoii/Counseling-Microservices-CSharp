using Common;

namespace AuthService.Application.Users.Queries.SelectToken;

public class SelectTokenQueryResponse : AbstractResponse<SelectTokenQueryEntity>
{
    public override SelectTokenQueryEntity Response { get; set; }
}

public class SelectTokenQueryEntity
{
    public string Role { get; set; } = null!;
}