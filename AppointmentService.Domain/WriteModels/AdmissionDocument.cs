namespace AppointmentService.Domain.WriteModels;

public partial class AdmissionDocument
{
    public Guid DocumentId { get; set; }

    public Guid? UploaderId { get; set; }

    public string Title { get; set; } = null!;

    public string? FilePath { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;
}
