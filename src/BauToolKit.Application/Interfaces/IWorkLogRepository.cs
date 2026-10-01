#nullable enable

using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Interfaces;

public interface IWorkLogRepository
{
    Task<IReadOnlyList<WorkLog>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<WorkLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkLog> AddAsync(WorkLog workLog, CancellationToken cancellationToken = default);
    Task<WorkLog> UpdateAsync(WorkLog workLog, CancellationToken cancellationToken = default);
}
