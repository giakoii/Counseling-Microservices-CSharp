using BuildingBlocks.CQRS;

namespace AuthService.Application.Users.Queries.SelectCounselorWithPaging;

public record SelectCounselorWithPagingQuery : IQuery<SelectCounselorWithPagingResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}