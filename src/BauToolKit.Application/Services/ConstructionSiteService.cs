#nullable enable

using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Enums;
using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Services;

public class ConstructionSiteService(
    IConstructionSiteRepository siteRepository,
    ICustomerRepository customerRepository) : IConstructionSiteService
{
    public async Task<IReadOnlyList<ConstructionSite>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await siteRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ConstructionSite>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await siteRepository.GetByCustomerIdAsync(customerId, cancellationToken);
    }

    public async Task<ConstructionSite?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await siteRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<ConstructionSite> CreateAsync(ConstructionSite site, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(site);
        if (string.IsNullOrWhiteSpace(site.Title))
        {
            throw new ArgumentException("Baustellentitel darf nicht leer sein.", nameof(site));
        }

        Customer? customer = await customerRepository.GetByIdAsync(site.CustomerId, cancellationToken);
        if (customer is null)
        {
            throw new InvalidOperationException($"Zugehöriger Kunde mit Id '{site.CustomerId}' existiert nicht.");
        }

        return await siteRepository.AddAsync(site, cancellationToken);
    }

    public async Task<ConstructionSite> UpdateProgressAsync(Guid siteId, int percentage, CancellationToken cancellationToken = default)
    {
        ConstructionSite? site = await siteRepository.GetByIdAsync(siteId, cancellationToken);
        if (site is null)
        {
            throw new KeyNotFoundException($"Baustelle mit Id '{siteId}' wurde nicht gefunden.");
        }

        site.UpdateProgress(percentage);
        return await siteRepository.UpdateAsync(site, cancellationToken);
    }

    public async Task<ConstructionSite> ChangeStatusAsync(Guid siteId, ConstructionSiteStatus status, CancellationToken cancellationToken = default)
    {
        ConstructionSite? site = await siteRepository.GetByIdAsync(siteId, cancellationToken);
        if (site is null)
        {
            throw new KeyNotFoundException($"Baustelle mit Id '{siteId}' wurde nicht gefunden.");
        }

        site.ChangeStatus(status);
        return await siteRepository.UpdateAsync(site, cancellationToken);
    }
}
