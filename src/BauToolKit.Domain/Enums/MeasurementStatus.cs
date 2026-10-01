#nullable enable

namespace BauToolKit.Domain.Enums;

/// <summary>
/// Status eines Aufmaß-/Messprojekts (Vorbereitung für 3D-Hausplaner).
/// </summary>
public enum MeasurementStatus
{
    Erfasst = 1,
    InBearbeitung = 2,
    Exportiert = 3
}
