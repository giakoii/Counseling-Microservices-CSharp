using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.WriteModels;

namespace AppointmentService.Application.Mappers;

public static class CounselorScheduleMapper
{
    public static CounselorScheduleCollection ToReadModel(CounselorSchedule schedule, string counselorName)
    {
        return new CounselorScheduleCollection
        {
            Id = Guid.NewGuid(), // Generated ID for read model
            CounselorName = counselorName,
            CounselorEmail = schedule.CounselorEmail,
            ScheduleDays = schedule.CounselorScheduleDays
                .Select(day => CounselorScheduleDayMapper.ToReadModel(day))
                .ToList(),
            CreatedAt = schedule.CreatedAt,
            CreatedBy = schedule.CreatedBy,
            IsActive = schedule.IsActive,
            UpdatedAt = schedule.UpdatedAt,
            UpdatedBy = schedule.UpdatedBy
        };
    }
}
