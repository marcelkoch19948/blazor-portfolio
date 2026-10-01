#nullable enable

namespace BauToolKit.Domain.Models;

/// <summary>
/// Einzelne Position auf einer Rechnung oder Abschlagsrechnung.
/// </summary>
public class InvoicePosition
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "Std"; // z. B. Std, Stk, pauschal, m
    public decimal UnitPrice { get; set; }

    public decimal LineTotal => Math.Round(Quantity * UnitPrice, 2, MidpointRounding.AwayFromZero);
}
