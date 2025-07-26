using AppointmentService.Domain.ReadModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using MassTransit.Initializers;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Helpers;

namespace AppointmentService.Application.CounselorSchedules.Queries;

public record SelectCounselorSchedulesQuery : IQuery<SelectCounselorSchedulesResponse>;

public class SelectCounselorSchedulesQueryHandler : IQueryHandler<SelectCounselorSchedulesQuery, SelectCounselorSchedulesResponse>
{
    private readonly INoSqlQueryRepository<CounselorScheduleDetailCollection> _counselorScheduleRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="counselorScheduleRepository"></param>
    public SelectCounselorSchedulesQueryHandler(INoSqlQueryRepository<CounselorScheduleDetailCollection> counselorScheduleRepository){
        _counselorScheduleRepository = counselorScheduleRepository;
    }

    /// <summary>
    /// Handles the SelectCounselorSchedulesQuery to retrieve counselor schedules.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<SelectCounselorSchedulesResponse> Handle(SelectCounselorSchedulesQuery request, CancellationToken cancellationToken)
    {
        var response = new SelectCounselorSchedulesResponse { Success = false };
        
        try
        {
            // Get current day of the week (1 = Monday, 7 = Sunday)
            var currentDayOfWeek = (int) DateTime.Now.DayOfWeek;
            
            // Convert to match your weekday system (assuming 1-7 where Monday = 1)
            var currentWeekdayId = currentDayOfWeek == 0 ? 7 : currentDayOfWeek;
            
            // Get current time
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            
            // Retrieve all counselor schedules
            var counselorSchedules = await _counselorScheduleRepository.FindAllAsync(x => x.IsActive);
            
            // Convert to a list of SelectCounselorSchedulesEntity
            var counselorScheduleEntities = counselorSchedules.Select(x => new SelectCounselorSchedulesEntity
            {
                CounselorEmail = x.Counselor.Email,
                CounselorName = $"{x.Counselor.FirstName} {x.Counselor.LastName}",
                DayId = x.WeekdayId,
                SlotId = x.SlotId,
                StatusId = x.StatusId,
                Day = x.DayName
            }).ToList();
            
            // Paginate the results
            var pagedResult = await PaginationHelper.PaginateAsync(counselorScheduleEntities);
            
            // Set the response
            response.Response = pagedResult;
            response.Success = true;
            response.SetMessage(MessageId.I00001);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.SetMessage(MessageId.E00000 ,$"Error retrieving counselor schedules: {ex.Message}");
        }
        
        return response;
    }
}

public class SelectCounselorSchedulesResponse : AbstractResponse<PagedResult<SelectCounselorSchedulesEntity>>
{
    public override PagedResult<SelectCounselorSchedulesEntity> Response { get; set; }
}

public class SelectCounselorSchedulesEntity
{
    public string CounselorEmail { get; set; }
    
    public string CounselorName { get; set; } = null!;

    public string Day { get; set; }
    
    public int DayId { get; set; }
    
    public string Slot { get; set; }
    
    public int SlotId { get; set; }
    
    public short? StatusId { get; set; }
}