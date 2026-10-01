#nullable enable

using BauToolKit.Application.DTOs;
using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Services;

public class InventoryService(
    IInventoryRepository inventoryRepository,
    IMaterialBookingRepository bookingRepository,
    IConstructionSiteRepository siteRepository) : IInventoryService
{
    public async Task<IReadOnlyList<InventoryItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        return await inventoryRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<InventoryItem> all = await inventoryRepository.GetAllAsync(cancellationToken);
        return all.Where(i => i.IsLowStock).ToList();
    }

    public async Task<InventoryItem?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await inventoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<InventoryItem> CreateItemAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (string.IsNullOrWhiteSpace(item.Name))
        {
            throw new ArgumentException("Artikelname darf nicht leer sein.", nameof(item));
        }

        return await inventoryRepository.AddAsync(item, cancellationToken);
    }

    public async Task<MaterialBooking> BookMaterialAsync(MaterialBookingRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Quantity <= 0)
        {
            throw new ArgumentException("Buchungsmenge muss positiv sein.", nameof(request));
        }

        InventoryItem? item = await inventoryRepository.GetByIdAsync(request.InventoryItemId, cancellationToken);
        if (item is null)
        {
            throw new KeyNotFoundException($"Lagerartikel mit Id '{request.InventoryItemId}' wurde nicht gefunden.");
        }

        if (request.ConstructionSiteId.HasValue)
        {
            ConstructionSite? site = await siteRepository.GetByIdAsync(request.ConstructionSiteId.Value, cancellationToken);
            if (site is null)
            {
                throw new KeyNotFoundException($"Baustelle mit Id '{request.ConstructionSiteId.Value}' wurde nicht gefunden.");
            }
        }

        var booking = new MaterialBooking
        {
            InventoryItemId = request.InventoryItemId,
            ConstructionSiteId = request.ConstructionSiteId,
            BookedByUserId = request.BookedByUserId,
            Type = request.Type,
            Quantity = request.Quantity,
            Notes = request.Notes
        };

        decimal delta = booking.CalculateStockDelta();
        item.AdjustStock(delta);

        await inventoryRepository.UpdateAsync(item, cancellationToken);
        return await bookingRepository.AddAsync(booking, cancellationToken);
    }

    public async Task<IReadOnlyList<MaterialBooking>> GetBookingsBySiteAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        return await bookingRepository.GetBySiteIdAsync(siteId, cancellationToken);
    }

    public async Task<IReadOnlyList<MaterialBooking>> GetBookingsByItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await bookingRepository.GetByItemIdAsync(itemId, cancellationToken);
    }
}
