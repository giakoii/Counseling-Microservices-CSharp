using System.Text.Json.Serialization;
using AppointmentService.Domain.WriteModels;

namespace AppointmentService.Domain.ReadModels;

public class AdmissionDocumentCollection
{
    [JsonPropertyName("id")]
    public Guid DocumentId { get; set; }
    
    [JsonPropertyName("uploader_id")]
    public Guid UploaderId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("file_path")]
    public string? FilePath { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("created_by")]
    public string CreatedBy { get; set; } = null!;

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("updated_by")]
    public string UpdatedBy { get; set; } = null!;

    public static AdmissionDocumentCollection FromWriteModel(AdmissionDocument doc)
    {
        var collection = new AdmissionDocumentCollection
        {
            DocumentId = doc.DocumentId,
            UploaderId = doc.UploaderId ?? Guid.Empty,
            Title = doc.Title,
            FilePath = doc.FilePath,
            Content = doc.Content,
            CreatedAt = doc.CreatedAt,
            CreatedBy = doc.CreatedBy,
            IsActive = doc.IsActive,
            UpdatedAt = doc.UpdatedAt,
            UpdatedBy = doc.UpdatedBy
        };
        return collection;
    }
}