using AppointmentService.Domain.ReadModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Shared.Application.Interfaces;

namespace AppointmentService.Application.CounselorSchedules.Queries;

public record SelectCounselorSchedulesQuery : IQuery<SelectCounselorSchedulesResponse>;

public class
    SelectCounselorSchedulesQueryHandler : IQueryHandler<SelectCounselorSchedulesQuery,
    SelectCounselorSchedulesResponse>
{
    private readonly INoSqlQueryRepository<CounselorScheduleDetailCollection> _counselorScheduleRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="counselorScheduleRepository"></param>
    public SelectCounselorSchedulesQueryHandler(INoSqlQueryRepository<CounselorScheduleDetailCollection> counselorScheduleRepository)
    {
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
            var now = DateTime.Now;
            var currentDayOfWeek = (int)now.DayOfWeek;
            var currentWeekdayId = currentDayOfWeek == 0 ? 7 : currentDayOfWeek;
            var currentTime = TimeOnly.FromDateTime(now);
    
            var counselorSchedules = await _counselorScheduleRepository.FindAllAsync(x => x.IsActive);
    
            List<CounselorScheduleDetailCollection> filteredSchedules;
    
            // If today is Sunday and after 5 PM, show all slots for next week (Monday to Sunday)
            if (currentDayOfWeek == 0 && currentTime > new TimeOnly(17, 0))
            {
                filteredSchedules = counselorSchedules
                    .Where(x => x.WeekdayId >= 1 && x.WeekdayId <= 7)
                    .ToList();
            }
            else
            {
                filteredSchedules = counselorSchedules
                    .Where(x =>
                        x.WeekdayId > currentWeekdayId ||
                        (x.WeekdayId == currentWeekdayId && x.StartTime > currentTime)
                    )
                    .ToList();
            }
    
            var counselorScheduleEntities = filteredSchedules.Select(x => new SelectCounselorSchedulesEntity
            {
                ScheduleId = x.Id,
                CounselorEmail = x.Counselor.Email,
                CounselorName = $"{x.Counselor.FirstName} {x.Counselor.LastName}",
                DayId = x.WeekdayId,
                SlotId = x.SlotId,
                StatusId = x.StatusId,
                Day = x.DayName,
                Slot = $"{x.StartTime} - {x.EndTime}",
            }).ToList();
    
            response.Response = counselorScheduleEntities;
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

public class SelectCounselorSchedulesResponse : AbstractResponse<List<SelectCounselorSchedulesEntity>>
{
    public override List<SelectCounselorSchedulesEntity> Response { get; set; }
}

public class SelectCounselorSchedulesEntity
{
    public Guid ScheduleId { get; set; }

    public string CounselorEmail { get; set; } = null!;

    public string CounselorName { get; set; } = null!;

    public string Day { get; set; }

    public int DayId { get; set; }

    public string Slot { get; set; }

    public int SlotId { get; set; }

    public short? StatusId { get; set; }
}