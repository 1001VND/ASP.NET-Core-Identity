using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using WebApp.Settings;

namespace WebApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<SMTPSetting> _smtpSetting;

        public EmailService(IOptions<SMTPSetting> smtpSetting)
        {
            _smtpSetting = smtpSetting;
        }

        public async Task SendAsync(string from, string to, string subject, string body)
        {
            var message = new MailMessage(from, to, subject, body);

            using (var emailClient = new SmtpClient(_smtpSetting.Value.Host, _smtpSetting.Value.Port))
            {
                emailClient.Credentials = new NetworkCredential(
                    _smtpSetting.Value.User,
                    _smtpSetting.Value.Password);
                await emailClient.SendMailAsync(message);
            }
        }
    }
}