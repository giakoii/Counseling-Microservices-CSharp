using AppointmentService.Application.CounselorSchedules.Queries.SelectCounselorScheduleById;
using AppointmentService.Domain.ReadModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Helpers;

namespace AppointmentService.Application.CounselorSchedules.Queries.SelectCounselorSchedulesByCounselorId;

/// <summary>
/// SelectCounselorSchedulesByCounselorIdQueryHandler - Handles the retrieval of counselor schedules by counselor ID.
/// </summary>
public class SelectCounselorSchedulesByCounselorIdQueryHandler : IQueryHandler<SelectCounselorSchedulesByCounselorIdQuery, SelectCounselorSchedulesByCounselorIdResponse>
{
    private readonly INoSqlQueryRepository<CounselorScheduleDetailCollection> _counselorScheduleRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="counselorScheduleRepository"></param>
    public SelectCounselorSchedulesByCounselorIdQueryHandler(INoSqlQueryRepository<CounselorScheduleDetailCollection> counselorScheduleRepository)
    {
        _counselorScheduleRepository = counselorScheduleRepository;
    }
    
    public async Task<SelectCounselorSchedulesByCounselorIdResponse> Handle(SelectCounselorSchedulesByCounselorIdQuery request, CancellationToken cancellationToken)
    {
        var response = new SelectCounselorSchedulesByCounselorIdResponse { Success = false };
        
        try
        {
            // Find counselor schedules by specific counselor ID
            var schedules = await _counselorScheduleRepository.FindAllAsync(x => x.CounselorId == request.CounselorId && x.IsActive);
            
            if (!schedules.Any())
            {
                response.SetMessage(MessageId.I00000, "No counselor schedules found for the specified counselor.");
                return response;
            }
            
            // Map schedules to response entities
            var scheduleEntities = schedules.Select(schedule => new SelectCounselorScheduleByIdEntity
            {
                ScheduleId = schedule.Id,
                CounselorId = schedule.CounselorId,
                CounselorEmail = schedule.Counselor.Email,
                CounselorName = $"{schedule.Counselor.FirstName} {schedule.Counselor.LastName}",
                DayId = schedule.WeekdayId,
                SlotId = schedule.SlotId,
                StatusId = schedule.StatusId,
                Day = schedule.DayName,
                Slot = $"{schedule.StartTime} - {schedule.EndTime}",
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                CreatedAt = schedule.CreatedAt,
                UpdatedAt = schedule.UpdatedAt,
                Counselor = schedule.Counselor
            }).OrderByDescending(x => x.DayId).ThenByDescending(x => x.StartTime).ToList();
            
            // Apply pagination
            var paginatedResult = await PaginationHelper.PaginateAsync(scheduleEntities, request.PageNumber, request.PageSize);
            
            response.Response = paginatedResult;
            response.Success = true;
            response.SetMessage(MessageId.I00001);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.SetMessage(MessageId.E00000, $"Error retrieving counselor schedules: {ex.Message}");
        }
        
        return response;
    }
}

public class SelectCounselorSchedulesByCounselorIdResponse : AbstractResponse<PagedResult<SelectCounselorScheduleByIdEntity>>
{
    public override PagedResult<SelectCounselorScheduleByIdEntity> Response { get; set; } = new();
}