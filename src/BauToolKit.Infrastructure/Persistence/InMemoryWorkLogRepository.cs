#nullable enable

using System.Collections.Concurrent;
using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Infrastructure.Persistence;

public class InMemoryWorkLogRepository : IWorkLogRepository
{
    private readonly ConcurrentDictionary<Guid, WorkLog> _workLogs = new();

    public Task<IReadOnlyList<WorkLog>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<WorkLog> list = _workLogs.Values
            .Where(w => w.ConstructionSiteId == siteId)
            .OrderByDescending(w => w.Date)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<IReadOnlyList<WorkLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<WorkLog> list = _workLogs.Values
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.Date)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<WorkLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _workLogs.TryGetValue(id, out WorkLog? workLog);
        return Task.FromResult(workLog);
    }

    public Task<WorkLog> AddAsync(WorkLog workLog, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(workLog);
        _workLogs[workLog.Id] = workLog;
        return Task.FromResult(workLog);
    }

    public Task<WorkLog> UpdateAsync(WorkLog workLog, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(workLog);
        _workLogs[workLog.Id] = workLog;
        return Task.FromResult(workLog);
    }
}
