#nullable enable

using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Services;

public class SiteAssignmentService(
    ISiteAssignmentRepository assignmentRepository,
    IUserRepository userRepository,
    IConstructionSiteRepository siteRepository) : ISiteAssignmentService
{
    public async Task<IReadOnlyList<SiteAssignment>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        return await assignmentRepository.GetBySiteIdAsync(siteId, cancellationToken);
    }

    public async Task<IReadOnlyList<SiteAssignment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await assignmentRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<SiteAssignment> AssignEmployeeAsync(
        Guid siteId,
        Guid userId,
        Guid? assignedByUserId,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        ConstructionSite? site = await siteRepository.GetByIdAsync(siteId, cancellationToken);
        if (site is null)
        {
            throw new KeyNotFoundException($"Baustelle mit Id '{siteId}' wurde nicht gefunden.");
        }

        User? user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new KeyNotFoundException($"Mitarbeiter mit Id '{userId}' wurde nicht gefunden.");
        }

        SiteAssignment? existing = await assignmentRepository.GetAsync(siteId, userId, cancellationToken);
        if (existing is not null)
        {
            if (existing.IsActive)
            {
                return existing;
            }

            existing.IsActive = true;
            existing.Notes = notes;
            return await assignmentRepository.UpdateAsync(existing, cancellationToken);
        }

        var assignment = new SiteAssignment
        {
            ConstructionSiteId = siteId,
            UserId = userId,
            AssignedByUserId = assignedByUserId,
            Notes = notes,
            IsActive = true
        };

        return await assignmentRepository.AddAsync(assignment, cancellationToken);
    }

    public async Task<bool> DeactivateAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        SiteAssignment? assignment = await assignmentRepository.GetByIdAsync(assignmentId, cancellationToken);
        if (assignment is null)
        {
            return false;
        }

        assignment.Deactivate();
        await assignmentRepository.UpdateAsync(assignment, cancellationToken);
        return true;
    }
}
