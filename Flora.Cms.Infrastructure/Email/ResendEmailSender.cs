using System.Net.Http.Json;
using Flora.Cms.Application.Abstractions.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Flora.Cms.Infrastructure.Email;

public sealed class ResendEmailSender(
    IConfiguration config,
    ILogger<ResendEmailSender> logger) : IEmailSender
{
    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        var apiKey = config["Contact:ResendApiKey"];
        var from = config["Contact:FromAddress"] ?? "Flora Group <noreply@floragroup.net>";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("Resend API key missing; skipping email to {To}", message.To);
            return;
        }

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var payload = new
        {
            from,
            to = new[] { message.To },
            subject = message.Subject,
            html = message.HtmlBody,
            reply_to = message.ReplyTo,
        };

        var res = await client.PostAsJsonAsync("https://api.resend.com/emails", payload, ct);
        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(ct);
            logger.LogError("Resend failed ({Status}): {Body}", res.StatusCode, body);
            throw new InvalidOperationException("Failed to send contact email.");
        }
    }
}
