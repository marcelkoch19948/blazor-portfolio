#nullable enable

using System.Collections.Concurrent;
using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Infrastructure.Persistence;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();

    public InMemoryUserRepository()
    {
    }

    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<User> list = _users.Values.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToList();
        return Task.FromResult(list);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _users.TryGetValue(id, out User? user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        User? user = _users.Values.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);
        _users[user.Id] = user;
        return Task.FromResult(user);
    }

    public Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);
        _users[user.Id] = user;
        return Task.FromResult(user);
    }
}
