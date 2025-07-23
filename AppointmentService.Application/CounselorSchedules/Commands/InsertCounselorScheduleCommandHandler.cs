using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.Snapshorts;
using AppointmentService.Domain.WriteModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Repositories;

namespace AppointmentService.Application.CounselorSchedules.Commands;

public record InsertCounselorScheduleCommand(Guid CounselorId, UserInformation Request) : ICommand<BaseCommandResponse>;

internal class InsertCounselorScheduleCommandHandler : ICommandHandler<InsertCounselorScheduleCommand, BaseCommandResponse>
{
    private readonly ICommandRepository<CounselorScheduleDetail> _counselorScheduleRepository;
    private readonly ICommandRepository<Weekday> _weekdayRepository;
    private readonly ICommandRepository<TimeSlot> _timeSlotRepository;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="weekdayRepository"></param>
    /// <param name="timeSlotRepository"></param>
    public InsertCounselorScheduleCommandHandler(ICommandRepository<Weekday> weekdayRepository, ICommandRepository<TimeSlot> timeSlotRepository, ICommandRepository<CounselorScheduleDetail> counselorScheduleRepository)
    {
        _weekdayRepository = weekdayRepository;
        _timeSlotRepository = timeSlotRepository;
        _counselorScheduleRepository = counselorScheduleRepository;
    }

    /// <summary>
    /// Handles the InsertCounselorScheduleCommand to insert a new counselor schedule.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<BaseCommandResponse> Handle(InsertCounselorScheduleCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseCommandResponse { Success = false };

        try
        {
            // Check to see if the counselor is scheduled.
            var existingSchedules = _counselorScheduleRepository
                .Find(cs => cs.CounselorId == request.CounselorId && cs.IsActive)
                .ToList();
            if (existingSchedules.Any())
            {
                response.SetMessage(MessageId.E00000, CommonMessages.CounselorScheduleExists);
                return response;
            }

            // Select all weekdays that are active
            var weekdays = await _weekdayRepository.Find().ToListAsync(cancellationToken: cancellationToken);

            // Select all available time slots
            var timeSlots = await _timeSlotRepository.Find().ToListAsync(cancellationToken: cancellationToken);
            
            var counselorSchedules = new List<CounselorScheduleDetail>();
            
            // Insert new counselor schedule
            await _counselorScheduleRepository.ExecuteInTransactionAsync(async () =>
            {
                foreach (var day in weekdays)
                {
                    foreach (var slot in timeSlots)
                    {
                        var counselorSchedule = new CounselorScheduleDetail
                        {
                            CounselorId = request.CounselorId,
                            WeekdayId = day!.Id,
                            SlotId = slot!.Id,
                        };
                        counselorSchedules.Add(counselorSchedule);
                    }
                }
                
                // Save changes
                await _counselorScheduleRepository.AddRangeAsync(counselorSchedules);
                await _counselorScheduleRepository.SaveChangesAsync("Admin");

                foreach (var scheduleDetail in counselorSchedules)
                {
                    _counselorScheduleRepository.Store(CounselorScheduleDetailCollection.FromWriteModel(scheduleDetail, request.Request), "Admin");
                }
                await _counselorScheduleRepository.SessionSavechanges();

                // True
                response.Success = true;
                response.SetMessage(MessageId.I00001);
                return true;
            });
           
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.SetMessage(MessageId.E99999);
        }
        
        return response;
    }
}