#nullable enable

using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Interfaces;

public interface IMaterialBookingRepository
{
    Task<IReadOnlyList<MaterialBooking>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MaterialBooking>> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task<MaterialBooking> AddAsync(MaterialBooking booking, CancellationToken cancellationToken = default);
}
