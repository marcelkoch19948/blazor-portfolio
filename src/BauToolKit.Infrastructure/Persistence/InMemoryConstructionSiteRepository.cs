#nullable enable

using System.Collections.Concurrent;
using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Infrastructure.Persistence;

public class InMemoryConstructionSiteRepository : IConstructionSiteRepository
{
    private readonly ConcurrentDictionary<Guid, ConstructionSite> _sites = new();

    public Task<IReadOnlyList<ConstructionSite>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<ConstructionSite> list = _sites.Values.OrderByDescending(s => s.CreatedAtUtc).ToList();
        return Task.FromResult(list);
    }

    public Task<IReadOnlyList<ConstructionSite>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<ConstructionSite> list = _sites.Values.Where(s => s.CustomerId == customerId).ToList();
        return Task.FromResult(list);
    }

    public Task<ConstructionSite?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _sites.TryGetValue(id, out ConstructionSite? site);
        return Task.FromResult(site);
    }

    public Task<ConstructionSite> AddAsync(ConstructionSite site, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(site);
        _sites[site.Id] = site;
        return Task.FromResult(site);
    }

    public Task<ConstructionSite> UpdateAsync(ConstructionSite site, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(site);
        _sites[site.Id] = site;
        return Task.FromResult(site);
    }
}
