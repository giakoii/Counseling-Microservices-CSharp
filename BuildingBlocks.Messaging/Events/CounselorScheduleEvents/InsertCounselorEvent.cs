namespace BuildingBlocks.Messaging.Events.CounselorScheduleEvents;

public record UserInformationRequest : IntegrationEvent
{
    public required Guid CounselorId { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; } = null!;
}