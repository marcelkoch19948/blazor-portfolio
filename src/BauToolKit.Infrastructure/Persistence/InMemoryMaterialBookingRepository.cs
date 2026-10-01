#nullable enable

using System.Collections.Concurrent;
using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Infrastructure.Persistence;

public class InMemoryMaterialBookingRepository : IMaterialBookingRepository
{
    private readonly ConcurrentBag<MaterialBooking> _bookings = new();

    public Task<IReadOnlyList<MaterialBooking>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<MaterialBooking> list = _bookings
            .Where(b => b.ConstructionSiteId == siteId)
            .OrderByDescending(b => b.BookingDateUtc)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<IReadOnlyList<MaterialBooking>> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<MaterialBooking> list = _bookings
            .Where(b => b.InventoryItemId == itemId)
            .OrderByDescending(b => b.BookingDateUtc)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<MaterialBooking> AddAsync(MaterialBooking booking, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(booking);
        _bookings.Add(booking);
        return Task.FromResult(booking);
    }
}
