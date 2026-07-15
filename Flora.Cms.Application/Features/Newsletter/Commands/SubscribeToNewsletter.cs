using FluentValidation;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Newsletter.Commands;

/// <summary>A visitor subscribes to corporate updates from the public site footer.</summary>
public sealed record SubscribeToNewsletterCommand(string Email, string Locale) : ICommand<Guid>;

public sealed class SubscribeToNewsletterCommandValidator : AbstractValidator<SubscribeToNewsletterCommand>
{
    public SubscribeToNewsletterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Locale).MaximumLength(8);
    }
}

/// <summary>
/// Idempotent: re-subscribing an existing email returns the existing id rather
/// than erroring. That keeps the public response identical for new and known
/// addresses, so the endpoint can't be used to probe which emails are subscribed.
/// </summary>
public sealed class SubscribeToNewsletterCommandHandler(IAppDbContext db, IDateTimeProvider clock)
    : ICommandHandler<SubscribeToNewsletterCommand, Guid>
{
    public async Task<Guid> Handle(SubscribeToNewsletterCommand command, CancellationToken ct)
    {
        var email = command.Email.Trim().ToLowerInvariant();

        var existing = await db.NewsletterSubscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Email == email, ct);

        if (existing is not null)
            return existing.Id;

        var subscription = new NewsletterSubscription
        {
            Email = email,
            Locale = string.IsNullOrWhiteSpace(command.Locale) ? "en" : command.Locale.Trim(),
            CreatedAtUtc = clock.UtcNow,
        };

        db.NewsletterSubscriptions.Add(subscription);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // Lost a check-then-insert race; the unique index on Email caught it.
            // The row now exists, which is all the caller asked for.
            var winner = await db.NewsletterSubscriptions
                .AsNoTracking()
                .FirstAsync(s => s.Email == email, ct);
            return winner.Id;
        }

        return subscription.Id;
    }
}
