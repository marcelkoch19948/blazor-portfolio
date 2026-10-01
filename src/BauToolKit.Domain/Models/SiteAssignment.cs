#nullable enable

namespace BauToolKit.Domain.Models;

/// <summary>
/// Zuweisung eines Mitarbeiters zu einer Baustelle durch den Chef.
/// </summary>
public class SiteAssignment
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ConstructionSiteId { get; set; }
    public Guid UserId { get; set; }
    public Guid? AssignedByUserId { get; set; }
    public DateTime AssignedAtUtc { get; init; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    public void Deactivate()
    {
        IsActive = false;
    }
}
