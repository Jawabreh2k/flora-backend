using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Jobs.Queries;

/// <summary>All companies, alphabetised — used to populate the admin job editor.</summary>
public sealed record GetCompaniesQuery : IQuery<IReadOnlyList<CompanyDto>>;

public sealed class GetCompaniesQueryHandler(IAppDbContext db)
    : IQueryHandler<GetCompaniesQuery, IReadOnlyList<CompanyDto>>
{
    public async Task<IReadOnlyList<CompanyDto>> Handle(GetCompaniesQuery query, CancellationToken ct)
        => await db.Companies
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CompanyDto(
                c.Id,
                c.Name,
                c.LogoUrl,
                c.Website,
                c.Description,
                c.CreatedAt,
                c.UpdatedAt))
            .ToListAsync(ct);
}
