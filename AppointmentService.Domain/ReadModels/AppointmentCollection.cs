namespace AppointmentService.Domain.ReadModels;

public class AppointmentCollection
{
    public Guid Id { get; set; }

    public Guid CounselorId { get; set; }
    
    public string CounselorName { get; set; } = null!;

    public Guid UserId { get; set; }
    
    public string UserName { get; set; } = null!;
    
    public string UserEmail { get; set; } = null!;

    public DateTime AppointmentDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }
    
    public string UpdatedBy { get; set; } = null!;
}