#nullable enable

namespace BauToolKit.Domain.Enums;

/// <summary>
/// Bearbeitungsstatus einer Rechnung.
/// </summary>
public enum InvoiceStatus
{
    Entwurf = 1,
    Gestellt = 2,
    Bezahlt = 3,
    Storniert = 4
}
