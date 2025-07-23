namespace AppointmentService.Domain.ReadModels;

public class TimeSlotCollection
{
    public int Id { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Display => $"{StartTime} - {EndTime}";
}