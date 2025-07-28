using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.Snapshorts;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Shared.Application.Interfaces;

namespace AppointmentService.Application.CounselorSchedules.Queries;

public class SelectCounselorScheduleByIdQuery : IQuery<SelectCounselorScheduleByIdResponse>
{
    public Guid ScheduleId { get; set; }
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
            response.Response = new SelectCounselorScheduleByIdEntity
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

public class SelectCounselorScheduleByIdResponse : AbstractResponse<SelectCounselorScheduleByIdEntity>
{
    public override SelectCounselorScheduleByIdEntity Response { get; set; } = null!;
}

public class SelectCounselorScheduleByIdEntity
{
    public Guid ScheduleId { get; set; }
    public Guid CounselorId { get; set; }
    public string CounselorEmail { get; set; } = null!;
    public string CounselorName { get; set; } = null!;
    public int DayId { get; set; }
    public int SlotId { get; set; }
    public short StatusId { get; set; }
    public string Day { get; set; } = null!;
    public string Slot { get; set; } = null!;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public UserInformation Counselor { get; set; } = null!;
}
