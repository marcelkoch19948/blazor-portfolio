#nullable enable

namespace BauToolKit.Application.DTOs;

public record WorkLogRequest(
    Guid UserId,
    Guid ConstructionSiteId,
    DateOnly Date,
    decimal Hours,
    string ActivityDescription);
