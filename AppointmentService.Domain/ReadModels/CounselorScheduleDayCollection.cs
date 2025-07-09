namespace AppointmentService.Domain.ReadModels;

public class CounselorScheduleDayCollection
{
    public Guid Id { get; set; }

    public int WeekDay { get; set; }

    public string WeekDayName { get; set; } = null!;

    public List<CounselorScheduleSlotCollection> Slots { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;
}