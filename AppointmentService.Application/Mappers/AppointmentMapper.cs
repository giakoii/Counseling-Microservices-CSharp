using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.WriteModels;

namespace AppointmentService.Application.Mappers;

public static class AppointmentMapper
{
    public static AppointmentCollection ToReadModel(Appointment appointment, string counselorName, string userName, string userEmail)
    {
        return new AppointmentCollection
        {
            Id = appointment.AppointmentId,
            CounselorId = appointment.CounselorId,
            CounselorName = counselorName,
            UserId = appointment.StudentId,
            UserName = userName,
            UserEmail = userEmail,
            AppointmentDate = appointment.AppointmentDate.ToDateTime(TimeOnly.MinValue),
            Status = appointment.StatusId?.ToString() ?? "Unknown",
            Notes = appointment.Content,
            CreatedAt = appointment.CreatedAt,
            CreatedBy = appointment.CreatedBy,
            IsActive = appointment.IsActive,
            UpdatedAt = appointment.UpdatedAt,
            UpdatedBy = appointment.UpdatedBy
        };
    }
}
