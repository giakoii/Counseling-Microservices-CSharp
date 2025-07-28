using Marten.Schema;

namespace RequestTicketService.Domain.Models;

public partial class RequestTicketChat
{
    [Identity]
    public Guid ChatId { get; set; }

    public Guid TicketId { get; set; }

    public Guid UserId { get; set; }

    public string Message { get; set; } = null!;

    public short? MessageTypeId { get; set; }

    public string? FileUrl { get; set; }

    public bool? IsInternal { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual RequestTicket Ticket { get; set; } = null!;
}
