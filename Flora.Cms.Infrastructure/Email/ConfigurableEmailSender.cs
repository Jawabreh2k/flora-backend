using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;
using Flora.Cms.Application.Abstractions.Email;
using Flora.Cms.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Flora.Cms.Infrastructure.Email;

/// <summary>
/// Single <see cref="IEmailSender"/> that resolves the admin-configured delivery settings
/// on each send and dispatches accordingly — so switching provider (Log/SMTP/Resend) in
/// the admin portal takes effect immediately, with no restart.
/// </summary>
public sealed class ConfigurableEmailSender(
    IEmailSettingsProvider settingsProvider,
    ILogger<ConfigurableEmailSender> logger) : IEmailSender
{
    // Reused across sends to avoid socket exhaustion (Resend transport).
    private static readonly HttpClient Http = new();

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        var s = await settingsProvider.GetResolvedAsync(ct);

        switch (s.Provider)
        {
            case EmailProvider.RESEND:
                await SendViaResendAsync(s, message, ct);
                break;
            case EmailProvider.SMTP:
                await SendViaSmtpAsync(s, message, ct);
                break;
            default:
                LogOnly(s, message);
                break;
        }
    }

    private void LogOnly(ResolvedEmailSettings s, EmailMessage message)
    {
        logger.LogInformation(
            "[Email:LOG] would send — from {From} to {To}: {Subject}\n{Body}",
            s.FromAddress, message.To, message.Subject, message.HtmlBody);
    }

    private async Task SendViaSmtpAsync(ResolvedEmailSettings s, EmailMessage message, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(s.SmtpHost))
        {
            logger.LogWarning("SMTP selected but host not configured — skipping email to {To}", message.To);
            return;
        }

        using var client = new SmtpClient(s.SmtpHost)
        {
            Port = s.SmtpPort,
            EnableSsl = s.SmtpUseSsl,
            Credentials = new NetworkCredential(s.SmtpUser, s.SmtpPassword),
        };

        using var mail = new MailMessage(s.FromAddress, message.To, message.Subject, message.HtmlBody)
        {
            IsBodyHtml = true,
        };

        if (!string.IsNullOrWhiteSpace(message.ReplyTo))
            mail.ReplyToList.Add(message.ReplyTo);

        await client.SendMailAsync(mail, ct);
    }

    private async Task SendViaResendAsync(ResolvedEmailSettings s, EmailMessage message, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(s.ResendApiKey))
        {
            logger.LogWarning("Resend selected but API key not configured — skipping email to {To}", message.To);
            return;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
        {
            Content = JsonContent.Create(new
            {
                from = s.FromAddress,
                to = new[] { message.To },
                subject = message.Subject,
                html = message.HtmlBody,
                reply_to = message.ReplyTo,
            }),
        };
        request.Headers.Add("Authorization", $"Bearer {s.ResendApiKey}");

        using var res = await Http.SendAsync(request, ct);
        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(ct);
            logger.LogError("Resend failed ({Status}): {Body}", res.StatusCode, body);
            throw new InvalidOperationException($"Resend rejected the message ({(int)res.StatusCode}): {body}");
        }
    }
}
