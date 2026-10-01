#nullable enable

using System.Collections.Concurrent;
using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Infrastructure.Persistence;

public class InMemorySiteAssignmentRepository : ISiteAssignmentRepository
{
    private readonly ConcurrentDictionary<Guid, SiteAssignment> _assignments = new();

    public Task<IReadOnlyList<SiteAssignment>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<SiteAssignment> list = _assignments.Values
            .Where(a => a.ConstructionSiteId == siteId && a.IsActive)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<IReadOnlyList<SiteAssignment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<SiteAssignment> list = _assignments.Values
            .Where(a => a.UserId == userId && a.IsActive)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<SiteAssignment?> GetAsync(Guid siteId, Guid userId, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SiteAssignment? assignment = _assignments.Values
            .FirstOrDefault(a => a.ConstructionSiteId == siteId && a.UserId == userId && (includeInactive || a.IsActive));
        return Task.FromResult(assignment);
    }

    public Task<SiteAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _assignments.TryGetValue(id, out SiteAssignment? assignment);
        return Task.FromResult(assignment);
    }

    public Task<SiteAssignment> AddAsync(SiteAssignment assignment, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(assignment);
        _assignments[assignment.Id] = assignment;
        return Task.FromResult(assignment);
    }

    public Task<SiteAssignment> UpdateAsync(SiteAssignment assignment, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(assignment);
        _assignments[assignment.Id] = assignment;
        return Task.FromResult(assignment);
    }
}
