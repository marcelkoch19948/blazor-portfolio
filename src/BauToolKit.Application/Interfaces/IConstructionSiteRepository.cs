#nullable enable

using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Interfaces;

public interface IConstructionSiteRepository
{
    Task<IReadOnlyList<ConstructionSite>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConstructionSite>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<ConstructionSite?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ConstructionSite> AddAsync(ConstructionSite site, CancellationToken cancellationToken = default);
    Task<ConstructionSite> UpdateAsync(ConstructionSite site, CancellationToken cancellationToken = default);
}
