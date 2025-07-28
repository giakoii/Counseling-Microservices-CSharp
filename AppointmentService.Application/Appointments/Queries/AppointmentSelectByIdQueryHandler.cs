using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.Snapshorts;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Shared.Application.Interfaces;

namespace AppointmentService.Application.Appointments.Queries;

public class AppointmentSelectByIdQuery : IQuery<AppointmentSelectByIdQueryResponse>
{
    public Guid AppointmentId { get; set; }
}

/// <summary>
/// AppointmentSelectByIdQueryHandler - Handles the retrieval of a single appointment by ID.
/// </summary>
public class AppointmentSelectByIdQueryHandler : IQueryHandler<AppointmentSelectByIdQuery, AppointmentSelectByIdQueryResponse>
{
    private readonly INoSqlQueryRepository<AppointmentCollection> _appointmentRepository;
    private readonly IIdentityService _identityService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appointmentRepository"></param>
    /// <param name="identityService"></param>
    public AppointmentSelectByIdQueryHandler(INoSqlQueryRepository<AppointmentCollection> appointmentRepository, IIdentityService identityService)
    {
        _appointmentRepository = appointmentRepository;
        _identityService = identityService;
    }
    
    public async Task<AppointmentSelectByIdQueryResponse> Handle(AppointmentSelectByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new AppointmentSelectByIdQueryResponse { Success = false };
        
        var currentUser = _identityService.GetCurrentUser();
        
        // Find specific appointment by ID
        var appointment = await _appointmentRepository.FindOneAsync(x => x.Id == request.AppointmentId && x.IsActive);
        
        if (appointment == null)
        {
            response.SetMessage(MessageId.I00000, "Appointment not found.");
            return response;
        }
        
        // Check if user has permission to view this appointment (either user or counselor)
        var userId = Guid.Parse(currentUser.UserId);
        if (appointment.UserId != userId && appointment.CounselorId != userId)
        {
            response.SetMessage(MessageId.E00000, "You don't have permission to view this appointment.");
            return response;
        }
        
        // Map appointment to response entity
        response.Response = new AppointmentSelectsQueryEntity
        {
            Id = appointment.Id,
            CounselorId = appointment.CounselorId,
            AppointmentDate = appointment.AppointmentDate,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt,
            Counselor = appointment.Counselor,
            User = appointment.User,
        };
        
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }
}

public class AppointmentSelectByIdQueryResponse : AbstractResponse<AppointmentSelectsQueryEntity>
{
    public override AppointmentSelectsQueryEntity Response { get; set; } = null!;
}
