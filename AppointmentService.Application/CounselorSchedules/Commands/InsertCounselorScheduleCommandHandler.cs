using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.Snapshorts;
using AppointmentService.Domain.WriteModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Interfaces;

namespace AppointmentService.Application.CounselorSchedules.Commands;

public record InsertCounselorScheduleCommand(UserInformation Counselor) : ICommand<BaseCommandResponse>;
public record InsertCounselorScheduleCommand(string CounselorEmail, string CounselorName)
    : ICommand<BaseResponse>;

internal class InsertCounselorScheduleCommandHandler : ICommandHandler<InsertCounselorScheduleCommand, BaseCommandResponse>
internal class InsertCounselorScheduleCommandHandler
    : ICommandHandler<InsertCounselorScheduleCommand, BaseResponse>
{
    private readonly ICommandRepository<CounselorScheduleDetail> _counselorScheduleRepository;
    private readonly ICommandRepository<Weekday> _weekdayRepository;
    private readonly ICommandRepository<TimeSlot> _timeSlotRepository;
    private readonly ICommandRepository<CounselorScheduleDay> _counselorScheduleDayRepository;
    private readonly ICommandRepository<CounselorScheduleSlot> _counselorScheduleSlotRepository;

    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="weekdayRepository"></param>
    /// <param name="timeSlotRepository"></param>
    /// <param name="counselorScheduleDayRepository"></param>
    /// <param name="counselorScheduleSlotRepository"></param>
    public InsertCounselorScheduleCommandHandler(
        ICommandRepository<CounselorSchedule> counselorScheduleRepository,
        ICommandRepository<Weekday> weekdayRepository,
        ICommandRepository<TimeSlot> timeSlotRepository,
        ICommandRepository<CounselorScheduleDay> counselorScheduleDayRepository,
        ICommandRepository<CounselorScheduleSlot> counselorScheduleSlotRepository
    )
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
    public async Task<BaseResponse> Handle(
        InsertCounselorScheduleCommand request,
        CancellationToken cancellationToken
    )
    {
        var response = new BaseCommandResponse { Success = false };

        try
        {
            // Check to see if the counselor is scheduled.
            var existingSchedules = _counselorScheduleRepository
                .Find(cs => cs.CounselorId == request.Counselor.Id && cs.IsActive)
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
            
            var timeSlots = await _timeSlotRepository.Find().ToListAsync();

            // Initialize lists for counselor schedule days and slots
            var counselorScheduleDays = new List<CounselorScheduleDay>();
            var counselorScheduleSlots = new List<CounselorScheduleSlot>();
            var counselorScheduleDayCollections = new List<CounselorScheduleDayCollection>();
            var counselorScheduleSlotCollections = new List<CounselorScheduleSlotCollection>();

            // Insert new counselor schedule
            await _counselorScheduleRepository.ExecuteInTransactionAsync(async () =>
            {
                foreach (var day in weekdays)
                {
                    var counselorScheduleDay = new CounselorScheduleDay
                    {
                        Id = Guid.NewGuid(),
                        CounselorEmail = request.CounselorEmail,
                        WeekdayId = day.Id,
                    };

                    foreach (var slot in timeSlots)
                    {
                        var counselorSchedule = new CounselorScheduleDetail
                        {
                            CounselorId = request.Counselor.Id,
                            WeekdayId = day!.Id,
                            SlotId = slot!.Id,
                            Status = ((short)ConstantEnum.ScheduleStatus.Available),
                        };
                        counselorSchedules.Add(counselorSchedule);
                        counselorScheduleSlots.Add(counselorScheduleSlot);
                        counselorScheduleSlotCollections.Add(
                            CounselorScheduleSlotMapper.ToReadModel(counselorScheduleSlot)
                        );
                    }

                    counselorScheduleDays.Add(counselorScheduleDay);
                    counselorScheduleDayCollections.Add(
                        CounselorScheduleDayMapper.ToReadModel(counselorScheduleDay)
                    );
                }
                

                // Add the new counselor schedule days and slots to the repository
                await _counselorScheduleDayRepository.AddRangeAsync(counselorScheduleDays);
                await _counselorScheduleSlotRepository.AddRangeAsync(counselorScheduleSlots);

                // Save changes
                await _counselorScheduleRepository.AddRangeAsync(counselorSchedules);
                await _counselorScheduleRepository.SaveChangesAsync("Admin");
                await _counselorScheduleRepository.SaveChangesAsync(request.CounselorEmail);

                _counselorScheduleRepository.Store(
                    CounselorScheduleMapper.ToReadModel(
                        newCounselorSchedule,
                        request.CounselorName
                    ),
                    "Admin"
                );
                _counselorScheduleDayRepository.StoreRange(
                    counselorScheduleDayCollections,
                    "Admin"
                );
                _counselorScheduleSlotRepository.StoreRange(
                    counselorScheduleSlotCollections,
                    "Admin"
                );

                foreach (var scheduleDetail in counselorSchedules)
                {
                    _counselorScheduleRepository.Store(CounselorScheduleDetailCollection.FromWriteModel(scheduleDetail, request.Counselor), "Admin");
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
