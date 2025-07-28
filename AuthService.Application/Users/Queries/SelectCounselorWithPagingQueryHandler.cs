using AuthService.Domain.ReadModels;
using BuildingBlocks.CQRS;
using BuildingBlocks.Messaging.Events.CounselorScheduleEvents;
using Common.Utils.Const;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Helpers;

namespace AuthService.Application.Users.Queries;

public record SelectCounselorWithPagingQuery : IQuery<SelectCounselorWithPagingResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class SelectCounselorWithPagingQueryHandler : IQueryHandler<SelectCounselorWithPagingQuery, SelectCounselorWithPagingResponse>
{
    private readonly INoSqlQueryRepository<UserCollection> _userRepository;

    public SelectCounselorWithPagingQueryHandler(INoSqlQueryRepository<UserCollection> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<SelectCounselorWithPagingResponse> Handle(SelectCounselorWithPagingQuery request, CancellationToken cancellationToken)
    {
        var response = new SelectCounselorWithPagingResponse { Success = false };
        
        try
        {
            var result = await _userRepository.FindAllAsync(x => x.IsActive);
            var counselors = result
                .Where(x => x.RoleInf?.Name == ConstantEnum.Role.Consultant.ToString())
                .Select(x => new CounselorWithId
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                })
                .ToList();
                
            if(!counselors.Any())
            {
                response.Message = "There are no counselors";
                return response;
            }

            // Apply pagination
            var paginatedResult = await PaginationHelper.PaginateAsync(counselors, request.PageNumber, request.PageSize);

            response.Response = paginatedResult;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }
}

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
