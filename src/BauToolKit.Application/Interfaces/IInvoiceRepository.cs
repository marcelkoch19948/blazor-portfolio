#nullable enable

using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Invoice> AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<Invoice> UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default);
}
