using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.WriteModels;

namespace AppointmentService.Application.Mappers;

public static class TimeSlotMapper
{
    public static TimeSlotCollection ToReadModel(TimeSlot timeSlot)
    {
        return new TimeSlotCollection
        {
            Id = timeSlot.Id,
            StartTime = timeSlot.StartTime,
            EndTime = timeSlot.EndTime
        };
    }
}
