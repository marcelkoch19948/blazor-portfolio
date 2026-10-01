#nullable enable

namespace BauToolKit.Domain.Models;

/// <summary>
/// Fotodokumentation einer Baustelle für Bautagebuch, Abnahmen und Baufortschritt.
/// </summary>
public class PhotoDocumentation
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ConstructionSiteId { get; set; }
    public Guid UploadedByUserId { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CapturedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
}
