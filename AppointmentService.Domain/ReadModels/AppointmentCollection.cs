using System.Text.Json.Serialization;
using AppointmentService.Domain.Snapshorts;
using AppointmentService.Domain.WriteModels;

namespace AppointmentService.Domain.ReadModels;

public class AppointmentCollection
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("counselor_id")]
    public Guid CounselorId { get; set; }
    
    [JsonPropertyName("user_id")]
    public Guid UserId { get; set; }
    
    [JsonPropertyName("appointment_date")]
    public DateOnly AppointmentDate { get; set; }

    [JsonPropertyName("status")]
    public short Status { get; set; }

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
    
    public UserInformation Counselor { get; set; } = null!;
    
    public UserInformation User { get; set; } = null!;
    
    public static AppointmentCollection FromWriteModel(Appointment model, UserInformation userInformation, bool includeRelated = false)
    {
        var result = new AppointmentCollection
        {
            Id = model.AppointmentId,
            CounselorId = model.CounselorId,
            AppointmentDate = model.AppointmentDate,
            Status = model.StatusId ?? 1,
            CreatedAt = model.CreatedAt,
            CreatedBy = model.CreatedBy,
            IsActive = model.IsActive,
            UpdatedAt = model.UpdatedAt,
            UpdatedBy = model.UpdatedBy,
            Counselor = userInformation
        };

        if (includeRelated)
        {
            result.User = userInformation;
        }

        return result;
    }
}