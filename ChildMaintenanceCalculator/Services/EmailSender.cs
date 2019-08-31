using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Services
{
    public interface IEmailSenderService
    {
        void SendEmail(string body, string emailTo);
    }

    public class EmailSenderService : IEmailSenderService
    {
        //TODO: Config the SMPT Values
        internal const string smtpServer = "smtp.gmail.com";
        internal const int smtpPort = 587;
        internal const string smtpUser = "childmaintenancecalculator@gmail.com";
        internal const string smtpPassword = "coffeebean62";
        internal const string emailFrom = "childmaintenancecalculator@gmail.com";
        internal const string emailSubject = "Your Calculation";

        //TODO: Can this be made to return a success flag?
        public void SendEmail(string body, string emailTo)
        {
            using (var mailMessage = new MailMessage())
            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                // configure the client and send the message
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                client.EnableSsl = true;

                // configure the mail message
                mailMessage.From = new MailAddress(emailFrom);
                mailMessage.To.Insert(0, new MailAddress(emailTo));
                mailMessage.Subject = emailSubject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                client.Send(mailMessage);
            }
        }
    }
}
