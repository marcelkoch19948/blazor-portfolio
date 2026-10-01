#nullable enable

using BauToolKit.Domain.Enums;

namespace BauToolKit.Domain.Models;

/// <summary>
/// Baustelle bzw. Bauprojekt eines Kunden mit aktuellem Status und Fortschritt.
/// </summary>
public class ConstructionSite
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public ConstructionSiteStatus Status { get; set; } = ConstructionSiteStatus.Geplant;
    public int ProgressPercentage { get; private set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? TargetCompletionDate { get; set; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public string FullAddress => $"{Street}, {ZipCode} {City}".Trim(',', ' ');

    public void UpdateProgress(int percentage)
    {
        if (percentage < 0 || percentage > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(percentage), "Baufortschritt muss zwischen 0 und 100 Prozent liegen.");
        }

        ProgressPercentage = percentage;
        if (percentage == 100 && Status != ConstructionSiteStatus.Abgeschlossen)
        {
            Status = ConstructionSiteStatus.Abgeschlossen;
        }
        else if (percentage > 0 && Status == ConstructionSiteStatus.Geplant)
        {
            Status = ConstructionSiteStatus.InArbeit;
        }
    }

    public void ChangeStatus(ConstructionSiteStatus newStatus)
    {
        Status = newStatus;
        if (newStatus == ConstructionSiteStatus.Abgeschlossen)
        {
            ProgressPercentage = 100;
        }
    }
}
