using MailKit.Security;
using MimeKit;
using SecondLife.Domain;
using SecondLife.Domain.DomainModels;
using SecondLife.Service.Interface;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SecondLife.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public void AddNewEmailMessage(EmailMessage email)
        {
            throw new NotImplementedException();
        }

        public async Task SendEmailAsync(EmailMessage email)
        {
            MimeMessage emailMessage = new MimeMessage()
            {
                Sender = new MailboxAddress(_emailSettings.SendersName, _emailSettings.SmtpUserName),
                Subject = email.Subject
            };

            emailMessage.From.Add(new MailboxAddress(_emailSettings.EmailDisplayName, _emailSettings.SmtpUserName));
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = email.Content };
            emailMessage.To.Add(new MailboxAddress(email.MailTo, email.MailTo));

            try
            {
                using (var stream = new MailKit.Net.Smtp.SmtpClient())
                {
                    var socketOption = _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
                    await stream.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpServerPort, socketOption);

                    if (!String.IsNullOrEmpty(_emailSettings.SmtpUserName))
                    {
                        await stream.AuthenticateAsync(_emailSettings.SmtpUserName, _emailSettings.SmtpPassword);
                    }

                    await stream.SendAsync(emailMessage);

                    await stream.DisconnectAsync(true);

                }
            }
            catch (SmtpException ex)
            {
                throw ex;
            }

            throw new NotImplementedException();
        }
    }
}
