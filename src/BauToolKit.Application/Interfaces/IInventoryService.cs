#nullable enable

using BauToolKit.Application.DTOs;
using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Interfaces;

public interface IInventoryService
{
    Task<IReadOnlyList<InventoryItem>> GetAllItemsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
    Task<InventoryItem?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventoryItem> CreateItemAsync(InventoryItem item, CancellationToken cancellationToken = default);
    Task<MaterialBooking> BookMaterialAsync(MaterialBookingRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MaterialBooking>> GetBookingsBySiteAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MaterialBooking>> GetBookingsByItemAsync(Guid itemId, CancellationToken cancellationToken = default);
}
