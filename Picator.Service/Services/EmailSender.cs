using Picator.Service.Contracts;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace Picator.Service.Services;

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string msg)
    {
        var message = new MimeMessage();
        //    message.From.Add (new MailboxAddress (_writableLocations.Value.EmailSetting.Sender, _writableLocations.Value.EmailSetting.Email));
        message.To.Add(new MailboxAddress("receiver", email));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = msg
        };

        using var client = new SmtpClient();
        //    await client.ConnectAsync (_writableLocations.Value.EmailSetting.Host, _writableLocations.Value.EmailSetting.Port, true);
        // Note: only needed if the SMTP server requires authentication
        //   await client.AuthenticateAsync(_writableLocations.Value.EmailSetting.Username, _writableLocations.Value.EmailSetting.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}