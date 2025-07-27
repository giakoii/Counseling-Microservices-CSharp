using System.Text.Json.Serialization;
using AppointmentService.Domain.Snapshorts;
using AppointmentService.Domain.WriteModels;

namespace AppointmentService.Domain.ReadModels;

public class CounselorScheduleDetailCollection
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("counselor_id")]
    public Guid CounselorId { get; set; }
    
    [JsonPropertyName("weekday_id")]
    public short WeekdayId { get; set; }
    
    [JsonPropertyName("day_name")]
    public string DayName { get; set; } = null!;

    [JsonPropertyName("slot_id")]
    public short SlotId { get; set; }
    
    [JsonPropertyName("start_time")]
    public TimeOnly StartTime { get; set; }
    
    [JsonPropertyName("end_time")]
    public TimeOnly EndTime { get; set; }
    
    [JsonPropertyName("status_id")]
    public short StatusId { get; set; }
    
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

    public static CounselorScheduleDetailCollection FromWriteModel(CounselorScheduleDetail model, UserInformation userInformation)
    {
        var result = new CounselorScheduleDetailCollection
        {
            Id = model.Id,
            CounselorId = model.CounselorId,
            CreatedAt = model.CreatedAt,
            CreatedBy = model.CreatedBy,
            IsActive = model.IsActive,
            UpdatedAt = model.UpdatedAt,
            UpdatedBy = model.UpdatedBy,
            Counselor = userInformation,
            WeekdayId = model.WeekdayId,
            DayName = model.Weekday.DayName,
            SlotId = model.SlotId,
            StartTime = model.Slot.StartTime,
            EndTime = model.Slot.EndTime,
            StatusId = model.Status,
        };

        return result;
    }
}