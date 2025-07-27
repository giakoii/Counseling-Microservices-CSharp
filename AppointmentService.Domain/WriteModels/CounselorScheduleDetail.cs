namespace AppointmentService.Domain.WriteModels;

public partial class CounselorScheduleDetail
{
    public Guid Id { get; set; }

    public Guid CounselorId { get; set; }

    public short WeekdayId { get; set; }

    public short SlotId { get; set; }

    public short Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual TimeSlot Slot { get; set; } = null!;

    public virtual Weekday Weekday { get; set; } = null!;
}
