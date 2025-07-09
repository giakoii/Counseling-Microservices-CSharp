namespace AppointmentService.Domain.ReadModels;

public class CounselorScheduleCollection
{
    public Guid Id { get; set; }
    
    public string CounselorEmail { get; set; } = null!;
    
    public string CounselorName { get; set; } = null!;

    public List<CounselorScheduleDayCollection> ScheduleDays { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;
}