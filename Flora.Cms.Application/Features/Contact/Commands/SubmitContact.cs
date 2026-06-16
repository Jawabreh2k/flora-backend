using FluentValidation;
using Flora.Cms.Application.Abstractions.Contact;
using Flora.Cms.Application.Abstractions.Email;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Domain.Entities;

namespace Flora.Cms.Application.Features.Contact.Commands;

public sealed record SubmitContactCommand(
    string Name,
    string Email,
    string? Company,
    string Subject,
    string Message,
    string Locale) : ICommand<Guid>;

public sealed class SubmitContactCommandValidator : AbstractValidator<SubmitContactCommand>
{
    public SubmitContactCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(256);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Company).MaximumLength(256);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(512);
        RuleFor(x => x.Message).NotEmpty().MinimumLength(10).MaximumLength(8000);
        RuleFor(x => x.Locale).MaximumLength(8);
    }
}

public sealed class SubmitContactCommandHandler(
    IAppDbContext db,
    IEmailSender emailSender,
    IContactRecipientResolver recipientResolver,
    IDateTimeProvider clock) : ICommandHandler<SubmitContactCommand, Guid>
{
    public async Task<Guid> Handle(SubmitContactCommand command, CancellationToken ct)
    {
        var inquiry = new ContactInquiry
        {
            Name = command.Name.Trim(),
            Email = command.Email.Trim(),
            Company = command.Company?.Trim(),
            Subject = command.Subject.Trim(),
            Message = command.Message.Trim(),
            Locale = string.IsNullOrWhiteSpace(command.Locale) ? "en" : command.Locale.Trim(),
            CreatedAtUtc = clock.UtcNow,
        };

        db.ContactInquiries.Add(inquiry);
        await db.SaveChangesAsync(ct);

        var recipient = await recipientResolver.ResolveAsync(inquiry.Locale, ct);

        var html = $"""
            <h2>New contact inquiry</h2>
            <p><strong>Name:</strong> {System.Net.WebUtility.HtmlEncode(inquiry.Name)}</p>
            <p><strong>Email:</strong> {System.Net.WebUtility.HtmlEncode(inquiry.Email)}</p>
            <p><strong>Company:</strong> {System.Net.WebUtility.HtmlEncode(inquiry.Company ?? "—")}</p>
            <p><strong>Subject:</strong> {System.Net.WebUtility.HtmlEncode(inquiry.Subject)}</p>
            <p><strong>Locale:</strong> {System.Net.WebUtility.HtmlEncode(inquiry.Locale)}</p>
            <hr />
            <p>{System.Net.WebUtility.HtmlEncode(inquiry.Message).Replace("\n", "<br/>")}</p>
            """;

        await emailSender.SendAsync(
            new EmailMessage(
                recipient,
                $"[Flora Group] {inquiry.Subject}",
                html,
                inquiry.Email),
            ct);

        return inquiry.Id;
    }
}
