using AppointmentService.Domain;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils;
using Common.Utils.Const;
using Shared.Application.Repositories;

namespace AppointmentService.Application.CounselorSchedules.Queries;

public record SelectCounselorSchedulesQuery() : IQuery<SelectCounselorSchedulesResponse>;

public class SelectCounselorSchedulesQueryHandler : IQueryHandler<SelectCounselorSchedulesQuery, SelectCounselorSchedulesResponse>
{
    private readonly INoSqlQueryRepository<CounselorSchedule> _counselorScheduleRepository;
    private readonly INoSqlQueryRepository<CounselorScheduleDay> _counselorScheduleDayRepository;
    private readonly INoSqlQueryRepository<CounselorScheduleSlot> _counselorScheduleSlotRepository;
    private readonly INoSqlQueryRepository<Weekday> _weekdayRepository;
    private readonly INoSqlQueryRepository<TimeSlot> _timeSlotRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="counselorScheduleRepository"></param>
    /// <param name="weekdayRepository"></param>
    /// <param name="timeSlotRepository"></param>
    /// <param name="counselorScheduleSlotRepository"></param>
    /// <param name="counselorScheduleDayRepository"></param>
    public SelectCounselorSchedulesQueryHandler(INoSqlQueryRepository<CounselorSchedule> counselorScheduleRepository, INoSqlQueryRepository<Weekday> weekdayRepository, INoSqlQueryRepository<TimeSlot> timeSlotRepository, INoSqlQueryRepository<CounselorScheduleSlot> counselorScheduleSlotRepository, INoSqlQueryRepository<CounselorScheduleDay> counselorScheduleDayRepository)
    {
        _counselorScheduleRepository = counselorScheduleRepository;
        _weekdayRepository = weekdayRepository;
        _timeSlotRepository = timeSlotRepository;
        _counselorScheduleSlotRepository = counselorScheduleSlotRepository;
        _counselorScheduleDayRepository = counselorScheduleDayRepository;
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
            
            var weekdays = await _weekdayRepository.FindAllAsync();
            
            // Get all time slots
            var timeSlots = await _timeSlotRepository.FindAllAsync();
            
            // Get schedule days for today and future days only
            var scheduleDays = await _counselorScheduleDayRepository.FindAllAsync(x => x.WeekdayId >= currentWeekdayId);
            
            // Filter schedule slots for available status (status = 1)
            var scheduleSlots = await _counselorScheduleSlotRepository.FindAllAsync(x => x.Status == 1);
            
            var scheduleEntities = new List<SelectCounselorSchedulesEntity>();
            
            foreach (var schedule in counselorSchedules)
            {
                // Get schedule days for this counselor (only today and future days)
                var counselorScheduleDays = scheduleDays.Where(x => x.CounselorEmail == schedule.CounselorEmail);
                
                foreach (var scheduleDay in counselorScheduleDays)
                {
                    // Get available time slots for this schedule day
                    var availableTimeSlots = scheduleSlots.Where(x => x.ScheduleDayId == scheduleDay.Id);
                    
                    foreach (var timeSlot in availableTimeSlots)
                    {
                        var weekdayName = weekdays.Find(x => x.Id == scheduleDay.WeekdayId)?.DayName;
                        var slotName = timeSlots.Find(x => x.Id == timeSlot.SlotId);
                        
                        if (slotName != null)
                        {
                            // For today: only include slots that haven't started yet
                            // For future days: include all slots
                            bool isToday = scheduleDay.WeekdayId == currentWeekdayId;
                            bool isValidSlot = !isToday || slotName.StartTime > currentTime;
                            
                            if (isValidSlot)
                            {
                                var scheduleEntity = new SelectCounselorSchedulesEntity
                                {
                                    CounselorEmail = schedule.CounselorEmail,
                                    DayId = scheduleDay.WeekdayId,
                                    Day = weekdayName!,
                                    SlotId = timeSlot.SlotId,
                                    Slot = $"{StringUtil.ConvertToHhMm(slotName.StartTime)} - {StringUtil.ConvertToHhMm(slotName.EndTime)}",
                                    StatusId = timeSlot.Status
                                };
                                scheduleEntities.Add(scheduleEntity);
                            }
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
    public string CounselorEmail { get; set; }

    public string Day { get; set; }
    
    public int DayId { get; set; }
    
    public string Slot { get; set; }
    
    public int SlotId { get; set; }
    
    public short? StatusId { get; set; }
}