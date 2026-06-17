using FluentValidation;
using Flora.Cms.Application.Abstractions.Email;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Security;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Application.Features.Email.Contracts;
using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Email.Commands;

/// <summary>Creates or updates the singleton email delivery settings.</summary>
public sealed record UpdateEmailSettingsCommand(UpdateEmailSettingsInput Input) : ICommand<EmailSettingsDto>;

public sealed class UpdateEmailSettingsCommandValidator : AbstractValidator<UpdateEmailSettingsCommand>
{
    public UpdateEmailSettingsCommandValidator()
    {
        RuleFor(x => x.Input.Provider).IsInEnum();
        RuleFor(x => x.Input.FromAddress).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Input.SmtpHost).MaximumLength(256);
        RuleFor(x => x.Input.SmtpPort).InclusiveBetween(1, 65535);
        RuleFor(x => x.Input.SmtpUser).MaximumLength(256);
    }
}

public sealed class UpdateEmailSettingsCommandHandler(
    IAppDbContext db,
    IEmailSecretProtector protector,
    ICurrentUser currentUser,
    IDateTimeProvider clock)
    : ICommandHandler<UpdateEmailSettingsCommand, EmailSettingsDto>
{
    public async Task<EmailSettingsDto> Handle(UpdateEmailSettingsCommand command, CancellationToken ct)
    {
        var input = command.Input;

        var entity = await db.EmailSettings.FirstOrDefaultAsync(x => x.Id == EmailSettings.SingletonId, ct);
        if (entity is null)
        {
            entity = new EmailSettings { Id = EmailSettings.SingletonId };
            db.EmailSettings.Add(entity);
        }

        entity.Provider = input.Provider;
        entity.FromAddress = input.FromAddress.Trim();
        entity.SmtpHost = Trimmed(input.SmtpHost);
        entity.SmtpPort = input.SmtpPort;
        entity.SmtpUseSsl = input.SmtpUseSsl;
        entity.SmtpUser = Trimmed(input.SmtpUser);

        // Leave-blank-to-keep: only re-encrypt when a new secret was actually supplied.
        if (!string.IsNullOrWhiteSpace(input.SmtpPassword))
            entity.SmtpPasswordEncrypted = protector.Protect(input.SmtpPassword);
        if (!string.IsNullOrWhiteSpace(input.ResendApiKey))
            entity.ResendApiKeyEncrypted = protector.Protect(input.ResendApiKey);

        entity.UpdatedAtUtc = clock.UtcNow;
        entity.UpdatedBy = currentUser.UserName ?? "unknown";

        await db.SaveChangesAsync(ct);

        return new EmailSettingsDto(
            entity.Provider,
            entity.FromAddress,
            entity.SmtpHost,
            entity.SmtpPort,
            entity.SmtpUseSsl,
            entity.SmtpUser,
            SmtpPasswordSet: !string.IsNullOrEmpty(entity.SmtpPasswordEncrypted),
            ResendApiKeySet: !string.IsNullOrEmpty(entity.ResendApiKeyEncrypted),
            entity.UpdatedAtUtc,
            entity.UpdatedBy);
    }

    private static string? Trimmed(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
