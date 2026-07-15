using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Companies.Queries;

/// <summary>A single company's detail, for the admin edit form. Returns null when not found.</summary>
public sealed record GetCompanyByIdQuery(Guid Id) : IQuery<CompanyDto?>;

public sealed class GetCompanyByIdQueryHandler(IAppDbContext db)
    : IQueryHandler<GetCompanyByIdQuery, CompanyDto?>
{
    public async Task<CompanyDto?> Handle(GetCompanyByIdQuery query, CancellationToken ct)
        => await db.Companies
            .AsNoTracking()
            .Where(c => c.Id == query.Id)
            .Select(c => new CompanyDto(c.Id, c.Name, c.LogoUrl, c.Website, c.Description, c.CreatedAt, c.UpdatedAt))
            .FirstOrDefaultAsync(ct);
}
