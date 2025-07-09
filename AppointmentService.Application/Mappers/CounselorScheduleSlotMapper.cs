using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.WriteModels;

namespace AppointmentService.Application.Mappers;

public static class CounselorScheduleSlotMapper
{
    public static CounselorScheduleSlotCollection ToReadModel(CounselorScheduleSlot slot)
    {
        return new CounselorScheduleSlotCollection
        {
            Id = slot.Id,
            TimeSlotId = slot.SlotId,
            StartTime = slot.Slot?.StartTime.ToString() ?? "Unknown",
            EndTime = slot.Slot?.EndTime.ToString() ?? "Unknown",
            IsAvailable = slot.Status == 1, // Assuming 1 means available
            CreatedAt = slot.CreatedAt,
            CreatedBy = slot.CreatedBy,
            IsActive = slot.IsActive,
            UpdatedAt = slot.UpdatedAt,
            UpdatedBy = slot.UpdatedBy
        };
    }
}
