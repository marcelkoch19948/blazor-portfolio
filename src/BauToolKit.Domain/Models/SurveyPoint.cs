#nullable enable

namespace BauToolKit.Domain.Models;

/// <summary>
/// Einzelner 3D-Messpunkt (X, Y, Z in Metern) eines Aufmaßes auf der Baustelle.
/// </summary>
public class SurveyPoint
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid MeasurementProjectId { get; set; }
    public string Label { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public int SequenceOrder { get; set; }
    public string? Notes { get; set; }
}
