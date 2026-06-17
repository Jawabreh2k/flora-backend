using FluentValidation;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.Contact.Contracts;
using Flora.Cms.Application.Features.Contact.Mapping;
using Flora.Cms.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NotFoundException = Flora.Cms.Application.Common.Exceptions.NotFoundException;

namespace Flora.Cms.Application.Features.Contact.Commands;

/// <summary>Triages an inquiry in the inbox — mark read/unread or archive.</summary>
public sealed record UpdateContactInquiryStatusCommand(Guid Id, ContactInquiryStatus Status) : ICommand<ContactInquiryDto>;

public sealed class UpdateContactInquiryStatusCommandValidator : AbstractValidator<UpdateContactInquiryStatusCommand>
{
    public UpdateContactInquiryStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

public sealed class UpdateContactInquiryStatusCommandHandler(IAppDbContext db)
    : ICommandHandler<UpdateContactInquiryStatusCommand, ContactInquiryDto>
{
    public async Task<ContactInquiryDto> Handle(UpdateContactInquiryStatusCommand command, CancellationToken ct)
    {
        var inquiry = await db.ContactInquiries.FirstOrDefaultAsync(i => i.Id == command.Id, ct);
        if (inquiry is null)
            throw new NotFoundException("Contact inquiry not found.");

        inquiry.Status = command.Status;
        await db.SaveChangesAsync(ct);

        return ContactInquiryMappings.ToDto(inquiry);
    }
}
