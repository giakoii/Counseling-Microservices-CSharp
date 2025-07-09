namespace BuildingBlocks.Messaging.Events.CounselorScheduleEvents;

public record InsertCounselorScheduleRequest : IntegrationEvent
{
    public required string CounselorEmail { get; set; }
    
    public required string CounselorName { get; set; }
}