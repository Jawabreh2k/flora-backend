using FluentValidation;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Microsoft.EntityFrameworkCore;
using NotFoundException = Flora.Cms.Application.Common.Exceptions.NotFoundException;

namespace Flora.Cms.Application.Features.Companies.Commands;

/// <summary>Updates an existing company's details. Job postings filed under it are untouched.</summary>
public sealed record UpdateCompanyCommand(
    Guid Id,
    string Name,
    string? LogoUrl,
    string? Website,
    string? Description) : ICommand<CompanyDto>;

public sealed class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(256);
        RuleFor(x => x.LogoUrl).MaximumLength(2048);
        RuleFor(x => x.Website).MaximumLength(2048);
        RuleFor(x => x.Description).MaximumLength(4000);
    }
}

public sealed class UpdateCompanyCommandHandler(IAppDbContext db, IDateTimeProvider clock)
    : ICommandHandler<UpdateCompanyCommand, CompanyDto>
{
    public async Task<CompanyDto> Handle(UpdateCompanyCommand command, CancellationToken ct)
    {
        var company = await db.Companies.FirstOrDefaultAsync(c => c.Id == command.Id, ct);
        if (company is null)
            throw new NotFoundException("Company not found.");

        company.Name = command.Name.Trim();
        company.LogoUrl = string.IsNullOrWhiteSpace(command.LogoUrl) ? null : command.LogoUrl.Trim();
        company.Website = string.IsNullOrWhiteSpace(command.Website) ? null : command.Website.Trim();
        company.Description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim();
        company.UpdatedAt = clock.UtcNow;

        await db.SaveChangesAsync(ct);

        return new CompanyDto(company.Id, company.Name, company.LogoUrl, company.Website, company.Description, company.CreatedAt, company.UpdatedAt);
    }
}
