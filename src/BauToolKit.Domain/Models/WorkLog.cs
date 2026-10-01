#nullable enable

namespace BauToolKit.Domain.Models;

/// <summary>
/// Arbeitszeit- und Stundeneintrag eines Mitarbeiters auf einer Baustelle.
/// </summary>
public class WorkLog
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid ConstructionSiteId { get; set; }
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public decimal Hours { get; private set; }
    public string ActivityDescription { get; set; } = string.Empty;
    public bool IsApprovedByChef { get; private set; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public void SetHours(decimal hours)
    {
        if (hours <= 0 || hours > 24)
        {
            throw new ArgumentOutOfRangeException(nameof(hours), "Arbeitsstunden müssen zwischen 0 und 24 Stunden liegen.");
        }

        Hours = hours;
    }

    public void Approve()
    {
        IsApprovedByChef = true;
    }

    public void Reject()
    {
        IsApprovedByChef = false;
    }
}
