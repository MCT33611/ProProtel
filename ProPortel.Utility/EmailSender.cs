using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace ProPortel.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailToSend = new MimeMessage();

            emailToSend.From.Add(MailboxAddress.Parse("proprotel04@gmail.com"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            //Send email

            using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
            {
                emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("proprotel04@gmail.com", "kewf nasc jfkv vsqw");
                emailClient.Send(emailToSend);
                emailClient.Disconnect(true);

            }

            return Task.CompletedTask;
        }
    }
}
