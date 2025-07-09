using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.WriteModels;

namespace AppointmentService.Application.Mappers;

public static class CounselorScheduleDayMapper
{
    public static CounselorScheduleDayCollection ToReadModel(CounselorScheduleDay scheduleDay)
    {
        return new CounselorScheduleDayCollection
        {
            Id = scheduleDay.Id,
            WeekDay = scheduleDay.WeekdayId,
            WeekDayName = scheduleDay.Weekday?.DayName!,
            Slots = scheduleDay.CounselorScheduleSlots
                .Select(slot => CounselorScheduleSlotMapper.ToReadModel(slot))
                .ToList(),
            CreatedAt = scheduleDay.CreatedAt,
            CreatedBy = scheduleDay.CreatedBy,
            IsActive = scheduleDay.IsActive,
            UpdatedAt = scheduleDay.UpdatedAt,
            UpdatedBy = scheduleDay.UpdatedBy
        };
    }
}
