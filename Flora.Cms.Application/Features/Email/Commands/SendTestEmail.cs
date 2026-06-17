using FluentValidation;
using Flora.Cms.Application.Abstractions.Email;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Features.Email.Contracts;
using Microsoft.Extensions.Logging;

namespace Flora.Cms.Application.Features.Email.Commands;

/// <summary>Sends a test message through the currently-saved delivery settings.</summary>
public sealed record SendTestEmailCommand(string To) : ICommand<TestEmailResultDto>;

public sealed class SendTestEmailCommandValidator : AbstractValidator<SendTestEmailCommand>
{
    public SendTestEmailCommandValidator()
    {
        RuleFor(x => x.To).NotEmpty().EmailAddress().MaximumLength(256);
    }
}

public sealed class SendTestEmailCommandHandler(
    IEmailSender emailSender,
    IEmailSettingsProvider settingsProvider,
    ILogger<SendTestEmailCommandHandler> logger)
    : ICommandHandler<SendTestEmailCommand, TestEmailResultDto>
{
    public async Task<TestEmailResultDto> Handle(SendTestEmailCommand command, CancellationToken ct)
    {
        var settings = await settingsProvider.GetResolvedAsync(ct);

        const string html =
            "<h2>Flora CMS — test email</h2>" +
            "<p>If you can read this, your email delivery settings are working.</p>";

        try
        {
            await emailSender.SendAsync(
                new EmailMessage(command.To.Trim(), "[Flora CMS] Test email", html),
                ct);

            return new TestEmailResultDto(true, settings.Provider, null);
        }
        catch (Exception ex)
        {
            // Surface the failure to the admin rather than turning it into a 500 — a failed
            // test is a normal, expected outcome while configuring credentials.
            logger.LogWarning(ex, "Test email to {To} failed via {Provider}", command.To, settings.Provider);
            return new TestEmailResultDto(false, settings.Provider, ex.Message);
        }
    }
}
