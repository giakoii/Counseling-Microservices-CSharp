namespace Shared.Application.Interfaces;

public interface ISendmailService
{ 
    void SendCounselorCreationEmail(string email, string password, SendmailConfig emailConfig);
}

public class SendmailConfig
{
    public string MailFrom { get; set; } = null!;
    
    public string MailPassword { get; set; } = null!;
}