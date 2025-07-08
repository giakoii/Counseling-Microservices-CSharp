namespace BuildingBlocks.Messaging.Events.InsertCounselorSchedule;

public record InsertCounselorScheduleRequest : IntegrationEvent
{
    public required string CounselorEmail { get; set; }
}