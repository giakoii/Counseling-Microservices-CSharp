using System.Net;
using System.Net.Mail;
using Shared.Application.Interfaces;

namespace Shared.Infrastructure.Logics;

public class SendmailService : ISendmailService
{
    public void SendCounselorCreationEmail(string email, string password, SendmailConfig emailConfig)
    {
        string body = $"<!DOCTYPE html><html><head><meta charset=\"UTF-8\"><title>Account Information</title></head>" +
                      $"<body style=\"margin:0;padding:0;background-color:#f4f4f4;\"><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\"" +
                      $" style=\"background-color:#f4f4f4;padding:40px 0;\"><tr><td align=\"center\"><table width=\"600\" " +
                      $"cellpadding=\"0\" cellspacing=\"0\" style=\"background-color:#ffffff;border-radius:8px;overflow:hidden;" +
                      $"font-family:Arial,sans-serif;\"><tr><td style=\"padding:30px 40px;text-align:center;background-color:#007bff;" +
                      $"color:#ffffff;font-size:24px;font-weight:bold;\">Account Created</td></tr><tr><td style=\"padding:30px 40px;" +
                      $"text-align:left;color:#333333;font-size:16px;\">Welcome to FPT Education Organization,<br><br>Your account has been successfully created." +
                      $" Below are your login credentials:</td></tr><tr>" +
                      $"<td style=\"padding:20px 40px;text-align:left;color:#000000;font-size:16px;\">" +
                      $"<strong>Email:</strong> {email}<br><strong>Password:</strong> {password}</td></tr><tr>" +
                      $"<td style=\"padding:30px 40px;text-align:left;color:#666666;font-size:14px;\">" +
                      $"Please keep this information secure.<br><br>Thanks,<br>FPT Education</td></tr></table></td></tr></table></body></html>";
        Send(email, "Account Created", body, emailConfig);
    }
    
    /// <summary>
    ///  Send mail
    /// </summary>
    /// <param name="mailAddress"></param>
    /// <param name="title"></param>
    /// <param name="body"></param>
    private static void Send(string mailAddress, string title, string body, SendmailConfig emailConfig)
    {
        var client = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(emailConfig.MailFrom, emailConfig.MailPassword)
        };

        // Generate a message instance and set parameters.
        using (var message = new MailMessage())
        {
            message.From = new MailAddress(emailConfig.MailFrom);
            message.To.Add(mailAddress);
            message.Subject = title;
            message.Body = body;
            message.IsBodyHtml = true;
            client.Send(message);
        }
    }

}