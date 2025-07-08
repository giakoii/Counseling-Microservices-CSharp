namespace BuildingBlocks.Messaging.Events;

public record IntegrationEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string EventType => GetType().AssemblyQualifiedName;
}