namespace AppointmentService.Domain.Snapshorts;

public class UserInformation
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
}