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

        public Task SendEmailAsync(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
