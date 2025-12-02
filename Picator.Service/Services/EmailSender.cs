using Picator.Service.Contracts;
using MailKit.Net.Smtp;
using MimeKit;

namespace Picator.Service.Services;

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string msg, bool isHtml = false)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(subject, "picator.technical@gmail.com"));
        message.To.Add(new MailboxAddress("receiver", email));
        message.Subject = subject;

        if (isHtml)
        {
            message.Body = new TextPart("html") { Text = msg };
        }
        else
        {
            message.Body = new TextPart("plain") { Text = msg };
        }

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        // Note: only needed if the SMTP server requires authentication
        await client.AuthenticateAsync("picator.technical@gmail.com", "zsgvknuufsqtwqjv");
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}