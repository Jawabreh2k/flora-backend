using System.Net;
using System.Net.Mail;
using Flora.Cms.Application.Abstractions.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Flora.Cms.Infrastructure.Email;

public sealed class SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        var host = config["Contact:SmtpHost"];
        var from = config["Contact:FromAddress"] ?? "noreply@floragroup.net";

        if (string.IsNullOrWhiteSpace(host))
        {
            logger.LogWarning(
                "SMTP not configured — contact email to {To}: {Subject}",
                message.To,
                message.Subject);
            return;
        }

        using var client = new SmtpClient(host)
        {
            Port = config.GetValue("Contact:SmtpPort", 587),
            EnableSsl = config.GetValue("Contact:SmtpUseSsl", true),
            Credentials = new NetworkCredential(
                config["Contact:SmtpUser"],
                config["Contact:SmtpPassword"]),
        };

        using var mail = new MailMessage(from, message.To, message.Subject, message.HtmlBody)
        {
            IsBodyHtml = true,
        };

        if (!string.IsNullOrWhiteSpace(message.ReplyTo))
            mail.ReplyToList.Add(message.ReplyTo);

        await client.SendMailAsync(mail, ct);
    }
}
