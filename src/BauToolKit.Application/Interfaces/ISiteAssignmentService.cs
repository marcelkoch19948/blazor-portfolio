#nullable enable

using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Interfaces;

public interface ISiteAssignmentService
{
    Task<IReadOnlyList<SiteAssignment>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SiteAssignment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<SiteAssignment> AssignEmployeeAsync(Guid siteId, Guid userId, Guid? assignedByUserId, string? notes = null, CancellationToken cancellationToken = default);
    Task<bool> DeactivateAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default);
}
