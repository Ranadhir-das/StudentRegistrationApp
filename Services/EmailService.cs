using System.Net;
using System.Net.Mail;

namespace StudentRegistrationApp.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config) => _config = config;

        public async Task SendEmailAsync(string toEmail, string subject, string body)
{
    var settings = _config.GetSection("EmailSettings");
    using var client = new SmtpClient(settings["SmtpServer"], int.Parse(settings["SmtpPort"]))
    {
        Credentials = new NetworkCredential(settings["SenderEmail"], settings["AppPassword"]),
        EnableSsl = true
    };

    var mailMessage = new MailMessage
    {
        From = new MailAddress(settings["SenderEmail"], "Student Portal Support"), // Adding a Display Name
        Subject = subject,
        Body = body,
        IsBodyHtml = true // CRITICAL: This enables the professional look
    };
    mailMessage.To.Add(toEmail);

    await client.SendMailAsync(mailMessage);
}
    }
}