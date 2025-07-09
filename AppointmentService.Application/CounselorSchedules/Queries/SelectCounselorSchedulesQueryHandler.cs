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
            // Retrieve all counselor schedules
            var counselorSchedules = await _counselorScheduleRepository.FindAllAsync(x => x.IsActive);
            
            var weekdays = await _weekdayRepository.FindAllAsync();
            var timeSlots = await _timeSlotRepository.FindAllAsync();
            
            var scheduleDays = await _counselorScheduleDayRepository.FindAllAsync();
            
            var scheduleSlots = await _counselorScheduleSlotRepository.FindAllAsync();
            
            var scheduleEntities = new List<SelectCounselorSchedulesEntity>();
            
            foreach (var schedule in counselorSchedules)
            {
                // Retrieve weekday and time slot details
                foreach (var scheduleDay in scheduleDays)
                {
                    var timeSlot = scheduleSlots.Find(x => x.ScheduleDayId == scheduleDay.Id);
                    var weekdayName = weekdays.Find(x => x.Id == scheduleDay.WeekdayId)?.DayName;
                    var slotName = timeSlots.Find(x => x.Id == timeSlot?.SlotId);
                    var scheduleEntity = new SelectCounselorSchedulesEntity
                    {
                        CounselorEmail = schedule.CounselorEmail,
                        DayId = scheduleDay.WeekdayId,
                        Day = weekdayName!,
                        SlotId = timeSlot!.SlotId,
                        Slot = $"{StringUtil.ConvertToHhMm(slotName!.StartTime)} - {StringUtil.ConvertToHhMm(slotName.EndTime)}",
                        StatusId = timeSlot.Status
                    };
                    scheduleEntities.Add(scheduleEntity);
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