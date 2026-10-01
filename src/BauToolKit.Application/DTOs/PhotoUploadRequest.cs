#nullable enable

namespace BauToolKit.Application.DTOs;

public record PhotoUploadRequest(
    Guid ConstructionSiteId,
    Guid UploadedByUserId,
    string FileName,
    Stream Content,
    string? Description = null);
