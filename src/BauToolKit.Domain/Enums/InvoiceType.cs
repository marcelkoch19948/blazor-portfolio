#nullable enable

namespace BauToolKit.Domain.Enums;

/// <summary>
/// Art der Rechnung (z. B. Abschlagsrechnung / Zwischenrechnung oder Schlussrechnung).
/// </summary>
public enum InvoiceType
{
    Abschlagsrechnung = 1,
    Schlussrechnung = 2,
    Einzelforderung = 3
}
