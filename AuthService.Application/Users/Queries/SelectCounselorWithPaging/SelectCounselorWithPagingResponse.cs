using Shared.Infrastructure.Helpers;

namespace AuthService.Application.Users.Queries.SelectCounselorWithPaging;

public class SelectCounselorWithPagingResponse
{
    public bool Success { get; set; }
    
    public string Message { get; set; } = null!;
    
    public PagedResult<CounselorWithId> Response { get; set; } = new();
}

public class CounselorWithId
{
    public Guid Id { get; set; }
    
    public string Email { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
}
