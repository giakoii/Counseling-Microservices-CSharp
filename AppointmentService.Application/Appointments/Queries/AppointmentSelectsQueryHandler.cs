using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.Snapshorts;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Shared.Application.Interfaces;

namespace AppointmentService.Application.Appointments.Queries;

public class AppointmentSelectsQuery : IQuery<AppointmentSelectsQueryResponse>
{
    
}

/// <summary>
/// AppointmentSelectsQueryHandler - Handles the retrieval of appointment selections.
/// </summary>
public class AppointmentSelectsQueryHandler : IQueryHandler<AppointmentSelectsQuery, AppointmentSelectsQueryResponse>
{
    private readonly INoSqlQueryRepository<AppointmentCollection> _appointmentRepository;
    private readonly IIdentityService _identityService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appointmentRepository"></param>
    /// <param name="identityService"></param>
    public AppointmentSelectsQueryHandler(INoSqlQueryRepository<AppointmentCollection> appointmentRepository, IIdentityService identityService)
    {
        _appointmentRepository = appointmentRepository;
        _identityService = identityService;
    }
    
    public async Task<AppointmentSelectsQueryResponse> Handle(AppointmentSelectsQuery request, CancellationToken cancellationToken)
    {
        var response = new AppointmentSelectsQueryResponse {Success = false};
        
        var currentUser = _identityService.GetCurrentUser();
        
        // Select all appointments by user
        var appointments = await _appointmentRepository.FindAllAsync(x => x.UserId == Guid.Parse(currentUser.UserId) && x.IsActive);
        if (!appointments.Any())
        {
            response.SetMessage(MessageId.I00000, "No appointments found for the user.");
            return response;
        }
        
        // Map appointments to response entities
        response.Response = appointments.Select(appointment => new AppointmentSelectsQueryEntity
        {
            Id = appointment.Id,
            CounselorId = appointment.CounselorId,
            AppointmentDate = appointment.AppointmentDate,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt,
            Counselor = appointment.Counselor,
            User = appointment.User,
        }).ToList();
        
        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }
}

public class AppointmentSelectsQueryResponse : AbstractResponse<List<AppointmentSelectsQueryEntity>>
{
    public override List<AppointmentSelectsQueryEntity> Response { get; set; }
}

public class AppointmentSelectsQueryEntity
{
    public Guid Id { get; set; }

    public Guid CounselorId { get; set; }
    
    public DateOnly AppointmentDate { get; set; }

    public short Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    
    public UserInformation Counselor { get; set; } = null!;
    
    public UserInformation User { get; set; } = null!;
}

