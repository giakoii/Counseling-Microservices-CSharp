using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.Snapshorts;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Helpers;

namespace AppointmentService.Application.Appointments.Queries;

public class AppointmentSelectsQuery : IQuery<AppointmentSelectsQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class AppointmentSelectByIdQuery : IQuery<AppointmentSelectByIdQueryResponse>
{
    public Guid AppointmentId { get; set; }
}

public class AppointmentSelectByUserIdQuery : IQuery<AppointmentSelectByUserIdQueryResponse>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// AppointmentSelectsQueryHandler - Handles the retrieval of appointment selections by user or counselor.
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
            // If no appointments found for user, check if the user is a counselor
            appointments = await _appointmentRepository.FindAllAsync(x => x.CounselorId == Guid.Parse(currentUser.UserId) && x.IsActive);
        }
        
        if (!appointments.Any())
        {
            response.SetMessage(MessageId.I00000, "No appointments found for the user.");
            return response;
        }
        
        // Map appointments to response entities
        var appointmentEntities = appointments.Select(appointment => new AppointmentSelectsQueryEntity
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
        
        // Apply pagination
        var paginatedResult = await PaginationHelper.PaginateAsync(appointmentEntities, request.PageNumber, request.PageSize);
        
        response.Response = paginatedResult;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }
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

/// <summary>
/// AppointmentSelectByUserIdQueryHandler - Handles the retrieval of appointments by specific user ID.
/// </summary>
public class AppointmentSelectByUserIdQueryHandler : IQueryHandler<AppointmentSelectByUserIdQuery, AppointmentSelectByUserIdQueryResponse>
{
    private readonly INoSqlQueryRepository<AppointmentCollection> _appointmentRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appointmentRepository"></param>
    public AppointmentSelectByUserIdQueryHandler(INoSqlQueryRepository<AppointmentCollection> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }
    
    public async Task<AppointmentSelectByUserIdQueryResponse> Handle(AppointmentSelectByUserIdQuery request, CancellationToken cancellationToken)
    {
        var response = new AppointmentSelectByUserIdQueryResponse { Success = false };
        
        try
        {
            // Find appointments by specific user ID
            var appointments = await _appointmentRepository.FindAllAsync(x => x.UserId == request.UserId && x.IsActive);
            
            if (!appointments.Any())
            {
                response.SetMessage(MessageId.I00000, "No appointments found for the specified user.");
                return response;
            }
            
            // Map appointments to response entities
            var appointmentEntities = appointments.Select(appointment => new AppointmentSelectsQueryEntity
            {
                Id = appointment.Id,
                CounselorId = appointment.CounselorId,
                AppointmentDate = appointment.AppointmentDate,
                Status = appointment.Status,
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt,
                Counselor = appointment.Counselor,
                User = appointment.User,
            }).OrderByDescending(x => x.AppointmentDate).ToList();
            
            // Apply pagination
            var paginatedResult = await PaginationHelper.PaginateAsync(appointmentEntities, request.PageNumber, request.PageSize);
            
            response.Response = paginatedResult;
            response.Success = true;
            response.SetMessage(MessageId.I00001);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.SetMessage(MessageId.E00000, $"Error retrieving appointments for user: {ex.Message}");
        }
        
        return response;
    }
}

public class AppointmentSelectsQueryResponse : AbstractResponse<PagedResult<AppointmentSelectsQueryEntity>>
{
    public override PagedResult<AppointmentSelectsQueryEntity> Response { get; set; } = new();
}

public class AppointmentSelectByIdQueryResponse : AbstractResponse<AppointmentSelectsQueryEntity>
{
    public override AppointmentSelectsQueryEntity Response { get; set; } = null!;
}

public class AppointmentSelectByUserIdQueryResponse : AbstractResponse<PagedResult<AppointmentSelectsQueryEntity>>
{
    public override PagedResult<AppointmentSelectsQueryEntity> Response { get; set; } = new();
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

