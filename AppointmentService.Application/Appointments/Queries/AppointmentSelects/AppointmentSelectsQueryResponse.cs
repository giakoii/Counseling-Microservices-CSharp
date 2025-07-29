using AppointmentService.Domain.Snapshorts;
using Common;
using Shared.Infrastructure.Helpers;

namespace AppointmentService.Application.Appointments.Queries.AppointmentSelects;

public class AppointmentSelectsQueryResponse : AbstractResponse<PagedResult<AppointmentSelectsQueryEntity>>
{
    public override PagedResult<AppointmentSelectsQueryEntity> Response { get; set; } = new();
}

public class AppointmentSelectsQueryEntity
{
    public Guid Id { get; set; }

    public Guid CounselorId { get; set; }
    
    public DateOnly AppointmentDate { get; set; }

    public short Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    
    public UserInformation Counselor { get; set; } = null!;
    
    public UserInformation User { get; set; } = null!;
    
    public short SlotId { get; set; }
    
    public string Slot { get; set; } = string.Empty;
    
    public short DayId { get; set; }

    public string Weekday { get; set; } = string.Empty;
}
