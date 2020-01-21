using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Rotativa.AspNetCore;

namespace ChildMaintenanceCalculator.Services
{
    public interface IEmailSenderService
    {
        bool SendEmail(string body, string emailTo, Attachment attachment = null);
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
        public bool SendEmail(string body, string emailTo, Attachment attachment = null)
        {
            try
            {
                using (var mailMessage = new MailMessage())
                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                    client.EnableSsl = true;

                    mailMessage.From = new MailAddress(emailFrom);
                    mailMessage.To.Insert(0, new MailAddress(emailTo));
                    mailMessage.Subject = emailSubject;
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
