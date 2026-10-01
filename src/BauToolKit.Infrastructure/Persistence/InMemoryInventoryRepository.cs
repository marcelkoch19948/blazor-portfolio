#nullable enable

using System.Collections.Concurrent;
using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Infrastructure.Persistence;

public class InMemoryInventoryRepository : IInventoryRepository
{
    private readonly ConcurrentDictionary<Guid, InventoryItem> _items = new();

    public Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<InventoryItem> list = _items.Values.OrderBy(i => i.Name).ToList();
        return Task.FromResult(list);
    }

    public Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _items.TryGetValue(id, out InventoryItem? item);
        return Task.FromResult(item);
    }

    public Task<InventoryItem> AddAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(item);
        _items[item.Id] = item;
        return Task.FromResult(item);
    }

    public Task<InventoryItem> UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(item);
        _items[item.Id] = item;
        return Task.FromResult(item);
    }
}
