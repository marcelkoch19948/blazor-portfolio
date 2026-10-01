#nullable enable

using System.Collections.Concurrent;
using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Infrastructure.Persistence;

public class InMemoryInvoiceRepository : IInvoiceRepository
{
    private readonly ConcurrentDictionary<Guid, Invoice> _invoices = new();

    public Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<Invoice> list = _invoices.Values
            .OrderByDescending(i => i.IssueDate)
            .ThenByDescending(i => i.CreatedAtUtc)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<IReadOnlyList<Invoice>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<Invoice> list = _invoices.Values
            .Where(i => i.ConstructionSiteId == siteId)
            .OrderByDescending(i => i.IssueDate)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<IReadOnlyList<Invoice>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<Invoice> list = _invoices.Values
            .Where(i => i.CustomerId == customerId)
            .OrderByDescending(i => i.IssueDate)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _invoices.TryGetValue(id, out Invoice? invoice);
        return Task.FromResult(invoice);
    }

    public Task<Invoice> AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(invoice);
        _invoices[invoice.Id] = invoice;
        return Task.FromResult(invoice);
    }

    public Task<Invoice> UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(invoice);
        _invoices[invoice.Id] = invoice;
        return Task.FromResult(invoice);
    }
}
