#nullable enable

using BauToolKit.Domain.Enums;

namespace BauToolKit.Domain.Models;

/// <summary>
/// Aufmaß- und Messprojekt auf einer Baustelle. Dient als datenseitiger Einstiegspunkt
/// für die spätere 3D-Hausplaner-Erweiterung (z. B. Three.js / WebGL-Visualisierung).
/// </summary>
public class MeasurementProject
{
    private readonly List<SurveyPoint> _points = new();

    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ConstructionSiteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MeasurementStatus Status { get; set; } = MeasurementStatus.Erfasst;
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public IReadOnlyList<SurveyPoint> Points => _points.AsReadOnly();

    public void AddPoint(SurveyPoint point)
    {
        ArgumentNullException.ThrowIfNull(point);
        _points.Add(point);
    }

    public void ClearPoints()
    {
        _points.Clear();
    }
}
