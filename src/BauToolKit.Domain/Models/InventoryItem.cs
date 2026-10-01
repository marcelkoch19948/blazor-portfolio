#nullable enable

namespace BauToolKit.Domain.Models;

/// <summary>
/// Lagerartikel mit aktuellem Bestand und Meldebestand.
/// </summary>
public class InventoryItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string ItemNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = "Stk"; // z. B. Stk, m, kg, m²
    public decimal CurrentStock { get; private set; }
    public decimal MinimumStock { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public bool IsLowStock => CurrentStock <= MinimumStock;

    public void AdjustStock(decimal quantityDelta)
    {
        if (CurrentStock + quantityDelta < 0)
        {
            throw new InvalidOperationException($"Lagerbestand für '{Name}' kann nicht negativ werden. Aktuell: {CurrentStock}, Änderung: {quantityDelta}");
        }

        CurrentStock += quantityDelta;
    }
}
