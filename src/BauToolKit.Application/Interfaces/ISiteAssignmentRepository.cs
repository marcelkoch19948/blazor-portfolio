#nullable enable

using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Interfaces;

public interface ISiteAssignmentRepository
{
    Task<IReadOnlyList<SiteAssignment>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SiteAssignment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<SiteAssignment?> GetAsync(Guid siteId, Guid userId, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<SiteAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SiteAssignment> AddAsync(SiteAssignment assignment, CancellationToken cancellationToken = default);
    Task<SiteAssignment> UpdateAsync(SiteAssignment assignment, CancellationToken cancellationToken = default);
}
