#nullable enable

using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Enums;
using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Services;

public class InvoiceService(
    IInvoiceRepository invoiceRepository,
    IConstructionSiteRepository siteRepository,
    ICustomerRepository customerRepository) : IInvoiceService
{
    public async Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await invoiceRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        return await invoiceRepository.GetBySiteIdAsync(siteId, cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await invoiceRepository.GetByCustomerIdAsync(customerId, cancellationToken);
    }

    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await invoiceRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
        {
            throw new ArgumentException("Rechnungsnummer darf nicht leer sein.", nameof(invoice));
        }

        ConstructionSite? site = await siteRepository.GetByIdAsync(invoice.ConstructionSiteId, cancellationToken);
        if (site is null)
        {
            throw new KeyNotFoundException($"Baustelle mit Id '{invoice.ConstructionSiteId}' wurde nicht gefunden.");
        }

        Customer? customer = await customerRepository.GetByIdAsync(invoice.CustomerId, cancellationToken);
        if (customer is null)
        {
            throw new KeyNotFoundException($"Kunde mit Id '{invoice.CustomerId}' wurde nicht gefunden.");
        }

        if (invoice.Positions.Count == 0)
        {
            throw new InvalidOperationException("Eine Rechnung muss mindestens eine Position enthalten.");
        }

        foreach (InvoicePosition position in invoice.Positions)
        {
            if (string.IsNullOrWhiteSpace(position.Description))
            {
                throw new ArgumentException("Positionsbeschreibung darf nicht leer sein.", nameof(invoice));
            }

            if (position.Quantity <= 0)
            {
                throw new ArgumentException("Menge einer Position muss größer als 0 sein.", nameof(invoice));
            }

            if (position.UnitPrice < 0)
            {
                throw new ArgumentException("Einheitspreis einer Position darf nicht negativ sein.", nameof(invoice));
            }
        }

        return await invoiceRepository.AddAsync(invoice, cancellationToken);
    }

    public async Task<Invoice> UpdateStatusAsync(Guid invoiceId, InvoiceStatus newStatus, CancellationToken cancellationToken = default)
    {
        Invoice? existing = await invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
        if (existing is null)
        {
            throw new KeyNotFoundException($"Rechnung mit Id '{invoiceId}' wurde nicht gefunden.");
        }

        existing.Status = newStatus;
        return await invoiceRepository.UpdateAsync(existing, cancellationToken);
    }
}
