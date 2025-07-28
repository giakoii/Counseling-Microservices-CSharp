using Marten.Schema;

namespace RequestTicketService.Domain.Models;

public partial class RequestTicket
{
    [Identity]
    public Guid TicketId { get; set; }

    public Guid StudentId { get; set; }

    public Guid? CounselorId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public short? PriorityId { get; set; }

    public string? Category { get; set; }

    public short? StatusId { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual ICollection<RequestTicketChat> RequestTicketChats { get; set; } =
        new List<RequestTicketChat>();
}
