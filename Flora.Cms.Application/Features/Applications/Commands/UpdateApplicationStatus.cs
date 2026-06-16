using FluentValidation;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.Applications.Contracts;
using Flora.Cms.Application.Features.Applications.Mapping;
using Flora.Cms.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NotFoundException = Flora.Cms.Application.Common.Exceptions.NotFoundException;

namespace Flora.Cms.Application.Features.Applications.Commands;

/// <summary>Moves an application along the hiring pipeline (screening, interviewing, …).</summary>
public sealed record UpdateApplicationStatusCommand(Guid Id, ApplicationStatus Status) : ICommand<ApplicationDto>;

public sealed class UpdateApplicationStatusCommandValidator : AbstractValidator<UpdateApplicationStatusCommand>
{
    public UpdateApplicationStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

public sealed class UpdateApplicationStatusCommandHandler(IAppDbContext db)
    : ICommandHandler<UpdateApplicationStatusCommand, ApplicationDto>
{
    public async Task<ApplicationDto> Handle(UpdateApplicationStatusCommand command, CancellationToken ct)
    {
        var application = await db.Applications.FirstOrDefaultAsync(a => a.Id == command.Id, ct);
        if (application is null)
            throw new NotFoundException("Application not found.");

        application.Status = command.Status;
        await db.SaveChangesAsync(ct);

        return ApplicationMappings.ToDto(application);
    }
}
