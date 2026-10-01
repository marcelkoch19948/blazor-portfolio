#nullable enable

using BauToolKit.Application.DTOs;
using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Interfaces;

public interface IWorkLogService
{
    Task<IReadOnlyList<WorkLog>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<WorkLog> LogHoursAsync(WorkLogRequest request, CancellationToken cancellationToken = default);
    Task<WorkLog> ApproveHoursAsync(Guid workLogId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalHoursForSiteAsync(Guid siteId, CancellationToken cancellationToken = default);
}
