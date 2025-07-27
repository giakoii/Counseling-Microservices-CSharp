using AppointmentService.Domain.ReadModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using MassTransit.Initializers;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Helpers;

namespace AppointmentService.Application.CounselorSchedules.Queries;

public class SelectCounselorSchedulesQuery : IQuery<SelectCounselorSchedulesResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class SelectCounselorScheduleByIdQuery : IQuery<SelectCounselorScheduleByIdResponse>
{
    public Guid ScheduleId { get; set; }
}

public class
    SelectCounselorSchedulesQueryHandler : IQueryHandler<SelectCounselorSchedulesQuery,
    SelectCounselorSchedulesResponse>
{
    private readonly INoSqlQueryRepository<CounselorScheduleDetailCollection> _counselorScheduleRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="counselorScheduleRepository"></param>
    public SelectCounselorSchedulesQueryHandler(
        INoSqlQueryRepository<CounselorScheduleDetailCollection> counselorScheduleRepository)
    {
        _counselorScheduleRepository = counselorScheduleRepository;
    }

    /// <summary>
    /// Handles the SelectCounselorSchedulesQuery to retrieve counselor schedules with pagination.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<SelectCounselorSchedulesResponse> Handle(SelectCounselorSchedulesQuery request, CancellationToken cancellationToken)
    {
        var response = new SelectCounselorSchedulesResponse { Success = false };
        try
        {
            // Retrieve all active counselor schedules first
            var counselorSchedules = await _counselorScheduleRepository.FindAllAsync(x => x.IsActive);

            if (!counselorSchedules.Any())
            {
                response.SetMessage(MessageId.I00000, "No counselor schedules found.");
                return response;
            }

            // Sort by weekday descending, then by start time descending
            var filteredSchedules = counselorSchedules
                .OrderByDescending(x => x.WeekdayId)
                .ThenByDescending(x => x.StartTime)
                .ToList();

            if (!filteredSchedules.Any())
            {
                response.SetMessage(MessageId.I00000, "No available counselor schedules found.");
                return response;
            }

            // Convert to a list of SelectCounselorSchedulesEntity
            var counselorScheduleEntities = filteredSchedules.Select(x => new SelectCounselorSchedulesEntity
            {
                ScheduleId = x.Id,
                CounselorId = x.CounselorId,
                CounselorEmail = x.Counselor.Email,
                CounselorName = $"{x.Counselor.FirstName} {x.Counselor.LastName}",
                DayId = x.WeekdayId,
                SlotId = x.SlotId,
                StatusId = x.StatusId,
                Day = x.DayName,
                Slot = $"{x.StartTime} - {x.EndTime}",
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Counselor = x.Counselor
            }).ToList();

            // Apply pagination
            var paginatedResult = await PaginationHelper.PaginateAsync(counselorScheduleEntities, request.PageNumber, request.PageSize);

            response.Response = paginatedResult;
            response.SetMessage(MessageId.I00001);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.SetMessage(MessageId.E00000, $"Error retrieving counselor schedules: {ex.Message}");
        }

        return response;
    }

    private int GetCurrentSlotId(TimeOnly currentTime)
    {
        // Define your slot end times (example: slot 1 ends at 8:30, slot 2 at 9:00, etc.)
        var slotEndTimes = new[]
        {
            new TimeOnly(8, 30),
            new TimeOnly(9, 0),
            new TimeOnly(9, 30),
            new TimeOnly(10, 0),
            new TimeOnly(10, 30),
            new TimeOnly(11, 0),
            new TimeOnly(11, 30),
            new TimeOnly(13, 0),
            new TimeOnly(13, 30),
            new TimeOnly(14, 0),
            new TimeOnly(14, 30),
            new TimeOnly(15, 0),
            new TimeOnly(15, 30),
            new TimeOnly(16, 0),
            new TimeOnly(16, 30),
            new TimeOnly(17, 0),
        };

        for (int i = 0; i < slotEndTimes.Length; i++)
        {
            if (currentTime < slotEndTimes[i])
                return i;
        }
        return slotEndTimes.Length;
    }
}

/// <summary>
/// SelectCounselorScheduleByIdQueryHandler - Handles the retrieval of a single counselor schedule by ID.
/// </summary>
public class SelectCounselorScheduleByIdQueryHandler : IQueryHandler<SelectCounselorScheduleByIdQuery, SelectCounselorScheduleByIdResponse>
{
    private readonly INoSqlQueryRepository<CounselorScheduleDetailCollection> _counselorScheduleRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="counselorScheduleRepository"></param>
    public SelectCounselorScheduleByIdQueryHandler(INoSqlQueryRepository<CounselorScheduleDetailCollection> counselorScheduleRepository)
    {
        _counselorScheduleRepository = counselorScheduleRepository;
    }
    
    public async Task<SelectCounselorScheduleByIdResponse> Handle(SelectCounselorScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new SelectCounselorScheduleByIdResponse { Success = false };
        
        try
        {
            // Find specific counselor schedule by ID
            var schedule = await _counselorScheduleRepository.FindOneAsync(x => x.Id == request.ScheduleId && x.IsActive);
            
            if (schedule == null)
            {
                response.SetMessage(MessageId.I00000, "Counselor schedule not found.");
                return response;
            }
            
            // Map schedule to response entity
            response.Response = new SelectCounselorSchedulesEntity
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
            };
            
            response.Success = true;
            response.SetMessage(MessageId.I00001);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.SetMessage(MessageId.E00000, $"Error retrieving counselor schedule: {ex.Message}");
        }
        
        return response;
    }
}

public class SelectCounselorSchedulesResponse : AbstractResponse<PagedResult<SelectCounselorSchedulesEntity>>
{
    public override PagedResult<SelectCounselorSchedulesEntity> Response { get; set; } = new();
}

public class SelectCounselorScheduleByIdResponse : AbstractResponse<SelectCounselorSchedulesEntity>
{
    public override SelectCounselorSchedulesEntity Response { get; set; } = null!;
}

public class SelectCounselorSchedulesEntity
{
    public Guid ScheduleId { get; set; }

    public Guid CounselorId { get; set; }

    public string CounselorEmail { get; set; } = null!;

    public string CounselorName { get; set; } = null!;

    public string Day { get; set; } = null!;

    public int DayId { get; set; }

    public string Slot { get; set; } = null!;

    public int SlotId { get; set; }

    public short? StatusId { get; set; }
    
    public TimeOnly StartTime { get; set; }
    
    public TimeOnly EndTime { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public AppointmentService.Domain.Snapshorts.UserInformation Counselor { get; set; } = null!;
}