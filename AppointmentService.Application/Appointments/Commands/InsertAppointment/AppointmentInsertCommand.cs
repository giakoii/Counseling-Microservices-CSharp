using System.ComponentModel.DataAnnotations;
using BuildingBlocks.CQRS;

namespace AppointmentService.Application.Appointments.Commands.InsertAppointment;

public class AppointmentInsertCommand : ICommand<AppointmentInsertResponse>
{
    [Required(ErrorMessage = "ScheduleId is required.")]
    public Guid ScheduleId { get; set; }
    
    [Required(ErrorMessage = "Content is required.")]
    [MaxLength(500, ErrorMessage = "Content cannot exceed 500 characters.")]
    public string Content { get; set; } = null!;
} 
