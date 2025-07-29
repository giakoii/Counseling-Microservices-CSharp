using AppointmentService.Domain.ReadModels;
using BuildingBlocks.CQRS;
using Common.Utils.Const;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Helpers;

namespace AppointmentService.Application.Appointments.Queries.AppointmentSelects;

/// <summary>
/// AppointmentSelectsQueryHandler - Handles the retrieval of all appointment selections.
/// </summary>
public class AppointmentSelectsQueryHandler : IQueryHandler<AppointmentSelectsQuery, AppointmentSelectsQueryResponse>
{
    private readonly INoSqlQueryRepository<AppointmentCollection> _appointmentRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appointmentRepository"></param>
    public AppointmentSelectsQueryHandler(INoSqlQueryRepository<AppointmentCollection> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }
    
    public async Task<AppointmentSelectsQueryResponse> Handle(AppointmentSelectsQuery request, CancellationToken cancellationToken)
    {
        var response = new AppointmentSelectsQueryResponse {Success = false};
        var allAppointments = await _appointmentRepository.FindAllAsync();
        var appointments = allAppointments.Where(x => x.IsActive).ToList();
        
        if (!appointments.Any())
        {
            response.SetMessage(MessageId.I00000, $"No active appointments found. Total appointments in DB: {allAppointments.Count}");
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
            SlotId = appointment.CounselorSchedule.SlotId,
            Slot = $"{appointment.CounselorSchedule.StartTime} - {appointment.CounselorSchedule.EndTime}",
            DayId = appointment.CounselorSchedule.WeekdayId,
            Weekday = appointment.CounselorSchedule.DayName,
        }).ToList();
        
        // Apply pagination
        var paginatedResult = await PaginationHelper.PaginateAsync(appointmentEntities, request.PageNumber, request.PageSize);
        
        response.Response = paginatedResult;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }
}


