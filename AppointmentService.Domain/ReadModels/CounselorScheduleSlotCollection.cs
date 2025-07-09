namespace AppointmentService.Domain.ReadModels;

public class CounselorScheduleSlotCollection
{
    public Guid Id { get; set; }

    public short TimeSlotId { get; set; }

    public string StartTime { get; set; } = null!;
    
    public string EndTime { get; set; } = null!;

    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;
}