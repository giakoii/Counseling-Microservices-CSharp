using AppointmentService.Domain.ReadModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Shared.Application.Repositories;

namespace AppointmentService.Application.CounselorSchedules.Queries;

public record SelectCounselorSchedulesQuery : IQuery<SelectCounselorSchedulesResponse>;

public class SelectCounselorSchedulesQueryHandler : IQueryHandler<SelectCounselorSchedulesQuery, SelectCounselorSchedulesResponse>
{
    private readonly INoSqlQueryRepository<CounselorScheduleCollection> _counselorScheduleRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="counselorScheduleRepository"></param>
    public SelectCounselorSchedulesQueryHandler(INoSqlQueryRepository<CounselorScheduleCollection> counselorScheduleRepository){
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
            var currentDayOfWeek = (int)DateTime.Now.DayOfWeek;
            
            // Convert to match your weekday system (assuming 1-7 where Monday = 1)
            var currentWeekdayId = currentDayOfWeek == 0 ? 7 : currentDayOfWeek;
            
            // Get current time
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            
            // Retrieve all counselor schedules
            var counselorSchedules = await _counselorScheduleRepository.FindAllAsync(x => x.IsActive);
            
            var scheduleEntities = new List<SelectCounselorSchedulesEntity>();
            
            foreach (var schedule in counselorSchedules)
            {
                // Loop through each day in the schedule
                foreach (var scheduleDay in schedule.ScheduleDays.Where(d => d.IsActive))
                {
                    // Loop through each slot in the day
                    foreach (var slot in scheduleDay.Slots.Where(s => s.IsActive))
                    {
                        // Parse slot start time
                        if (!TimeOnly.TryParse(slot.StartTime, out var slotStartTime))
                            continue;
                        
                        // Filter logic: only include slots that are in the future
                        bool shouldIncludeSlot = false;
                        
                        if (scheduleDay.WeekDay > currentWeekdayId)
                        {
                            // Future day - include all slots
                            shouldIncludeSlot = true;
                        }
                        else if (scheduleDay.WeekDay == currentWeekdayId)
                        {
                            // Same day - only include slots after current time
                            shouldIncludeSlot = slotStartTime > currentTime;
                        }
                        // scheduleDay.WeekDay < currentWeekdayId means past day - skip
                        
                        if (shouldIncludeSlot)
                        {
                            var entityResponse = new SelectCounselorSchedulesEntity
                            {
                                CounselorEmail = schedule.CounselorEmail,
                                CounselorName = schedule.CounselorName,
                                Day = scheduleDay.WeekDayName,
                                DayId = scheduleDay.WeekDay,
                                Slot = $"{slot.StartTime} - {slot.EndTime}",
                                SlotId = slot.TimeSlotId,
                                StatusId = slot.IsAvailable ? (short?)1 : (short?)0 // Assuming 1 = Available, 0 = Not Available
                            };
                            
                            scheduleEntities.Add(entityResponse);
                        }
                    }
                }
            }
            
            // Set the response
            response.Response = scheduleEntities;
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

public class SelectCounselorSchedulesResponse : AbstractResponse<List<SelectCounselorSchedulesEntity>>
{
    public override List<SelectCounselorSchedulesEntity> Response { get; set; }
}

public class SelectCounselorSchedulesEntity
{
    public required string CounselorEmail { get; set; }
    
    public string CounselorName { get; set; } = null!;

    public required string Day { get; set; }
    
    public int DayId { get; set; }
    
    public required string Slot { get; set; }
    
    public int SlotId { get; set; }
    
    public short? StatusId { get; set; }
}