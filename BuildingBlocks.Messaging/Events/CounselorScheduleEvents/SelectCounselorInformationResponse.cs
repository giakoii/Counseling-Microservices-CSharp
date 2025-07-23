namespace BuildingBlocks.Messaging.Events.CounselorScheduleEvents;

public class SelectCounselorInformationResponse
{
    public bool Success { get; set; }
    
    public string Message { get; set; } = null!;
    
    public List<CounselorInformation> Response { get; set; }
}

public class CounselorInformation
{
    public string Email { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
}