using AuthService.Domain.ReadModels;
using BuildingBlocks.CQRS;
using BuildingBlocks.Messaging.Events.CounselorScheduleEvents;
using Common.Utils.Const;
using Shared.Application.Interfaces;

namespace AuthService.Application.Users.Queries.SelectCounselor;

public class SelectCounselorQueryHandler : IQueryHandler<SelectCounselorQuery, SelectCounselorInformationResponse>
{
    private readonly INoSqlQueryRepository<UserCollection> _userRepository;

    public SelectCounselorQueryHandler(INoSqlQueryRepository<UserCollection> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<SelectCounselorInformationResponse> Handle(SelectCounselorQuery request, CancellationToken cancellationToken)
    {
        var response = new SelectCounselorInformationResponse { Success = false };
        
        try
        {
            var result = await _userRepository.FindAllAsync(x => x.IsActive);
            var counselors = result
                .Where(x => x.RoleInf?.Name == ConstantEnum.Role.Consultant.ToString())
                .Select(x => new CounselorInformation
                {
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

            response.Response = counselors;
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