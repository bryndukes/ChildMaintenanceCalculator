using System;
using System.Net;
using System.Net.Mail;
using ChildMaintenanceCalculator.Configuration;
using Microsoft.Extensions.Options;

namespace ChildMaintenanceCalculator.Services
{
    public interface IEmailSenderService
    {
        bool SendEmail(string body, string emailTo, Attachment attachment = null);
    }

    public class EmailSenderService : IEmailSenderService
    {
        private EmailSettings _settings;
        
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly string _emailFrom;
        private readonly string _emailSubject;

        public EmailSenderService(IOptions<EmailSettings> emailSettings)
        {
            this._settings = emailSettings.Value;
            this._smtpServer = _settings.SmtpServer;
            this._smtpPort = _settings.SmtpPort;
            this._smtpUser = _settings.SmtpUser;
            this._smtpPassword = _settings.SmtpPassword;
            this._emailFrom = _settings.EmailFrom;
            this._emailSubject = _settings.EmailSubject;

        }

        //TODO: Can this be made to return a success flag?
        public bool SendEmail(string body, string emailTo, Attachment attachment = null)
        {
            try
            {
                using (var mailMessage = new MailMessage())
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                    client.EnableSsl = true;

                    mailMessage.From = new MailAddress(_emailFrom);
                    mailMessage.To.Insert(0, new MailAddress(emailTo));
                    mailMessage.Subject = _emailSubject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    if(attachment != null)
                        mailMessage.Attachments.Add(attachment);

                    client.Send(mailMessage);
                    return true;
                }
            }
            catch (Exception e)
            {
                //TODO: Log exception Details
                return false;
            }

        }
    }
}
