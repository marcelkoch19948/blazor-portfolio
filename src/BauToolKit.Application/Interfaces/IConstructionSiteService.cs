#nullable enable

using BauToolKit.Domain.Enums;
using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Interfaces;

public interface IConstructionSiteService
{
    Task<IReadOnlyList<ConstructionSite>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConstructionSite>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<ConstructionSite?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ConstructionSite> CreateAsync(ConstructionSite site, CancellationToken cancellationToken = default);
    Task<ConstructionSite> UpdateProgressAsync(Guid siteId, int percentage, CancellationToken cancellationToken = default);
    Task<ConstructionSite> ChangeStatusAsync(Guid siteId, ConstructionSiteStatus status, CancellationToken cancellationToken = default);
}
