using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfig _emailConfig;
        public EmailSender(EmailConfig emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task SendEmailAsync(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendEmailAsync(emailMessage);

        }
        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = string.Format("<h2 style='color:red;'>{0}</h2>",message.Content)
            };
            if (message.Attachements!=null && message.Attachements.Any())
            {
                message.Attachements.Select((v, i) => new { item = v, index = (i+1) }).ToList()
                .ForEach(a=> 
                {
                    bodyBuilder.Attachments.Add($"attachment{a.index}",a.item);
                });
            }
            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private async Task SendEmailAsync(MimeMessage emailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    //ssl type connection to true
                    await client.ConnectAsync(_emailConfig.SmtpServer,_emailConfig.Port,true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.UserName,_emailConfig.Password);
                    await client.SendAsync(emailMessage);
                }
                catch (Exception ex)
                {
                    //This needs to be logged actually
                    await Console.Out.WriteLineAsync("" + ex.Message);
                    throw;
                }
                finally 
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
