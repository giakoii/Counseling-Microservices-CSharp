namespace BuildingBlocks.Messaging.Events.CounselorScheduleEvents;

public record SelectCounselorScheduleEvent : IntegrationEvent
{
    public List<Counselor> Counselors { get; set; } = new List<Counselor>();
}

public class Counselor
{
    public string Email { get; set; } = null!;
}