#nullable enable

using BauToolKit.Domain.Enums;

namespace BauToolKit.Domain.Models;

/// <summary>
/// Rechnung (z. B. Zwischen-/Abschlagsrechnung oder Schlussrechnung) für ein Bauprojekt.
/// Hinweis: Dies dient der betrieblichen Erfassung und Vorbereitung; es wird bewusst keine
/// komplexe Rechnungs-/Steuer-Compliance vorgetäuscht.
/// </summary>
public class Invoice
{
    private readonly List<InvoicePosition> _positions = new();

    public Guid Id { get; init; } = Guid.NewGuid();
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid ConstructionSiteId { get; set; }
    public Guid CustomerId { get; set; }
    public InvoiceType Type { get; set; } = InvoiceType.Abschlagsrechnung;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Entwurf;
    public DateOnly IssueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14));
    public decimal TaxRatePercentage { get; set; } = 19.0m;
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public IReadOnlyList<InvoicePosition> Positions => _positions.AsReadOnly();

    public decimal CalculateNetTotal()
    {
        return Math.Round(_positions.Sum(p => p.LineTotal), 2, MidpointRounding.AwayFromZero);
    }

    public decimal CalculateTaxAmount()
    {
        decimal net = CalculateNetTotal();
        return Math.Round(net * (TaxRatePercentage / 100m), 2, MidpointRounding.AwayFromZero);
    }

    public decimal CalculateGrossTotal()
    {
        return CalculateNetTotal() + CalculateTaxAmount();
    }

    public void AddPosition(InvoicePosition position)
    {
        ArgumentNullException.ThrowIfNull(position);
        _positions.Add(position);
    }

    public void RemovePosition(Guid positionId)
    {
        _positions.RemoveAll(p => p.Id == positionId);
    }
}
