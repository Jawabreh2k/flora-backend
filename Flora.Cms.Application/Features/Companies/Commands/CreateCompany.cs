using FluentValidation;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Flora.Cms.Domain.Entities;

namespace Flora.Cms.Application.Features.Companies.Commands;

/// <summary>Creates a company that job postings can be filed under.</summary>
public sealed record CreateCompanyCommand(
    string Name,
    string? LogoUrl,
    string? Website,
    string? Description) : ICommand<CompanyDto>;

public sealed class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(256);
        RuleFor(x => x.LogoUrl).MaximumLength(2048);
        RuleFor(x => x.Website).MaximumLength(2048);
        RuleFor(x => x.Description).MaximumLength(4000);
    }
}

public sealed class CreateCompanyCommandHandler(IAppDbContext db, IDateTimeProvider clock)
    : ICommandHandler<CreateCompanyCommand, CompanyDto>
{
    public async Task<CompanyDto> Handle(CreateCompanyCommand command, CancellationToken ct)
    {
        var now = clock.UtcNow;
        var company = new Company
        {
            Name = command.Name.Trim(),
            LogoUrl = string.IsNullOrWhiteSpace(command.LogoUrl) ? null : command.LogoUrl.Trim(),
            Website = string.IsNullOrWhiteSpace(command.Website) ? null : command.Website.Trim(),
            Description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.Companies.Add(company);
        await db.SaveChangesAsync(ct);

        return new CompanyDto(company.Id, company.Name, company.LogoUrl, company.Website, company.Description, company.CreatedAt, company.UpdatedAt);
    }
}
