using AppointmentService.Application.Appointments.Queries.AppointmentSelects;
using AppointmentService.Domain.ReadModels;
using BuildingBlocks.CQRS;
using Common.Utils.Const;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Helpers;

namespace AppointmentService.Application.Appointments.Queries.AppointmentSelectByUserId;



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


