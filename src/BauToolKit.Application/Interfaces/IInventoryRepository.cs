#nullable enable

using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Interfaces;

public interface IInventoryRepository
{
    Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventoryItem> AddAsync(InventoryItem item, CancellationToken cancellationToken = default);
    Task<InventoryItem> UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default);
}
