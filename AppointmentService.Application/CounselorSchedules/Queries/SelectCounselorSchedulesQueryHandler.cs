// using AppointmentService.Domain;
// using BuildingBlocks.CQRS;
// using Common;
// using Common.Utils;
// using Common.Utils.Const;
// using Shared.Application.Repositories;
//
// namespace AppointmentService.Application.CounselorSchedules.Queries;
//
// public record SelectCounselorSchedulesQuery() : IQuery<SelectCounselorSchedulesResponse>;
//
// public class SelectCounselorSchedulesQueryHandler : IQueryHandler<SelectCounselorSchedulesQuery, SelectCounselorSchedulesResponse>
// {
//     private readonly INoSqlQueryRepository<CounselorSchedule> _counselorScheduleRepository;
//     private readonly INoSqlQueryRepository<Weekday> _weekdayRepository;
//     private readonly INoSqlQueryRepository<TimeSlot> _timeSlotRepository;
//     
//     /// <summary>
//     /// Constructor
//     /// </summary>
//     /// <param name="counselorScheduleRepository"></param>
//     /// <param name="weekdayTimeSlotRepository"></param>
//     /// <param name="weekdayRepository"></param>
//     /// <param name="timeSlotRepository"></param>
//     public SelectCounselorSchedulesQueryHandler(INoSqlQueryRepository<CounselorSchedule> counselorScheduleRepository, INoSqlQueryRepository<Weekday> weekdayRepository, INoSqlQueryRepository<TimeSlot> timeSlotRepository)
//     {
//         _counselorScheduleRepository = counselorScheduleRepository;
//         _weekdayRepository = weekdayRepository;
//         _timeSlotRepository = timeSlotRepository;
//     }
//
//     /// <summary>
//     /// Handles the SelectCounselorSchedulesQuery to retrieve counselor schedules.
//     /// </summary>
//     /// <param name="request"></param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     public async Task<SelectCounselorSchedulesResponse> Handle(SelectCounselorSchedulesQuery request, CancellationToken cancellationToken)
//     {
//         var response = new SelectCounselorSchedulesResponse { Success = false };
//         
//         try
//         {
//             // Retrieve all counselor schedules
//             var counselorSchedules = await _counselorScheduleRepository.FindAllAsync(x => x.IsActive);
//             var weekdays = await _weekdayRepository.FindAllAsync();
//             var timeSlots = await _timeSlotRepository.FindAllAsync();
//             
//             var scheduleEntities = new List<SelectCounselorSchedulesEntity>();
//             
//             foreach (var schedule in counselorSchedules)
//             {
//                 // Retrieve weekday and time slot details
//                 var weekday = weekdays.Find(x => x.Id == schedule.DayId);
//                 var timeSlot = timeSlots.Find(x => x.Id == schedule.SlotId);
//                 
//                 var scheduleEntity = new SelectCounselorSchedulesEntity
//                 {
//                     ScheduleId = schedule.ScheduleId,
//                     CounselorId = schedule.CounselorId,
//                     DayId = schedule.DayId,
//                     Day = weekday!.DayName,
//                     SlotId = schedule.SlotId,
//                     Slot = $"{StringUtil.ConvertToHhMm(timeSlot!.StartTime)} - {StringUtil.ConvertToHhMm(timeSlot.EndTime)}",
//                     StatusId = schedule.StatusId
//                 };
//                 scheduleEntities.Add(scheduleEntity);
//             }
//             
//             // Set the response
//             response.Responses = scheduleEntities;
//             response.Success = true;
//             response.SetMessage(MessageId.I00001);
//         }
//         catch (Exception ex)
//         {
//             response.Success = false;
//             response.SetMessage($"Error retrieving counselor schedules: {ex.Message}");
//         }
//         
//         return response;
//     }
// }
//
// public class SelectCounselorSchedulesResponse : AbstractResponse<List<SelectCounselorSchedulesEntity>>
// {
//     public override List<SelectCounselorSchedulesEntity> Response { get; set; } = new();
//     
//     public List<SelectCounselorSchedulesEntity> Responses 
//     { 
//         get => Response; 
//         set => Response = value; 
//     }
// }
//
// public class SelectCounselorSchedulesEntity
// {
//     public Guid ScheduleId { get; set; }
//
//     public Guid CounselorId { get; set; }
//
//     public string Day { get; set; }
//     
//     public short DayId { get; set; }
//     
//     public string Slot { get; set; }
//     
//     public int SlotId { get; set; }
//     
//     public short? StatusId { get; set; }
// }