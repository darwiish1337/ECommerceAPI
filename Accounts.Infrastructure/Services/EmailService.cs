using Accounts.Application.Interfaces.Services;
using Accounts.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Accounts.Infrastructure.Services;

public class EmailService(ILogger<EmailService> logger, EmailSettings settings) : IEmailService
{
    public async Task SendVerificationCodeAsync(string email, string code, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = "Verification Code";
        message.Body = new TextPart("plain")
        {
            Text = $"Your verification code is: {code}"
        };

        try
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(settings.SmtpServer, settings.Port, settings.UseSsl, cancellationToken);
            await smtp.AuthenticateAsync(settings.SenderEmail, settings.Password, cancellationToken);
            await smtp.SendAsync(message, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Verification code sent to {Email}", email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email}", email);
            throw;
        }
    }
}