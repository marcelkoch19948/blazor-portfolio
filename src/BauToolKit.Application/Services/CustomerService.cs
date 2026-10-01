#nullable enable

using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Services;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await customerRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await customerRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer);
        if (string.IsNullOrWhiteSpace(customer.Name))
        {
            throw new ArgumentException("Kundenname darf nicht leer sein.", nameof(customer));
        }

        return await customerRepository.AddAsync(customer, cancellationToken);
    }

    public async Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer);
        if (string.IsNullOrWhiteSpace(customer.Name))
        {
            throw new ArgumentException("Kundenname darf nicht leer sein.", nameof(customer));
        }

        Customer? existing = await customerRepository.GetByIdAsync(customer.Id, cancellationToken);
        if (existing is null)
        {
            throw new KeyNotFoundException($"Kunde mit Id '{customer.Id}' wurde nicht gefunden.");
        }

        return await customerRepository.UpdateAsync(customer, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await customerRepository.DeleteAsync(id, cancellationToken);
    }
}
