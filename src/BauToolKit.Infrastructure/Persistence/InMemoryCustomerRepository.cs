#nullable enable

using System.Collections.Concurrent;
using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Infrastructure.Persistence;

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly ConcurrentDictionary<Guid, Customer> _customers = new();

    public Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<Customer> list = _customers.Values.OrderBy(c => c.Name).ToList();
        return Task.FromResult(list);
    }

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _customers.TryGetValue(id, out Customer? customer);
        return Task.FromResult(customer);
    }

    public Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(customer);
        _customers[customer.Id] = customer;
        return Task.FromResult(customer);
    }

    public Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(customer);
        _customers[customer.Id] = customer;
        return Task.FromResult(customer);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        bool removed = _customers.TryRemove(id, out _);
        return Task.FromResult(removed);
    }
}
