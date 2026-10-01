#nullable enable

using BauToolKit.Domain.Enums;
using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Interfaces;

public interface IInvoiceService
{
    Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<Invoice> UpdateStatusAsync(Guid invoiceId, InvoiceStatus newStatus, CancellationToken cancellationToken = default);
}
