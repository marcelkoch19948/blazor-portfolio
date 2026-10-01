#nullable enable

using BauToolKit.Domain.Enums;

namespace BauToolKit.Domain.Models;

/// <summary>
/// Buchung von Material (Zugang, Baustellenverbrauch oder Rückgabe).
/// </summary>
public class MaterialBooking
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid InventoryItemId { get; set; }
    public Guid? ConstructionSiteId { get; set; }
    public Guid BookedByUserId { get; set; }
    public BookingType Type { get; set; } = BookingType.Verbrauch;
    public decimal Quantity { get; set; }
    public DateTime BookingDateUtc { get; init; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    /// <summary>
    /// Berechnet die Bestandsänderung im Lager basierend auf der Buchungsart.
    /// Zugang: +Menge, Verbrauch: -Menge, Rückgabe: +Menge.
    /// </summary>
    public decimal CalculateStockDelta()
    {
        if (Quantity <= 0)
        {
            throw new InvalidOperationException("Buchungsmenge muss positiv sein.");
        }

        return Type switch
        {
            BookingType.Zugang => Quantity,
            BookingType.Verbrauch => -Quantity,
            BookingType.Rueckgabe => Quantity,
            _ => throw new InvalidOperationException($"Unbekannte Buchungsart: {Type}")
        };
    }
}
