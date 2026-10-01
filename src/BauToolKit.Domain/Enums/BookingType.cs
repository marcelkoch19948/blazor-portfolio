#nullable enable

namespace BauToolKit.Domain.Enums;

/// <summary>
/// Art einer Materialbuchung im Lager oder auf einer Baustelle.
/// </summary>
public enum BookingType
{
    Zugang = 1,
    Verbrauch = 2,
    Rueckgabe = 3
}
