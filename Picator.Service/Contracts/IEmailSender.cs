namespace Picator.Service.Contracts;

/// <summary>
/// Email sender service.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Send email.
    /// </summary>
    /// <param name="email">Email.</param>
    /// <param name="subject">Email subject.</param>
    /// <param name="message">Email content.</param>
    /// <returns></returns>
    Task SendEmailAsync(string email, string subject, string message);
}