#nullable enable

using BauToolKit.Application.DTOs;
using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Interfaces;

public interface IPhotoDocumentationService
{
    Task<IReadOnlyList<PhotoDocumentation>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<PhotoDocumentation> AddPhotoAsync(PhotoUploadRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeletePhotoAsync(Guid photoId, CancellationToken cancellationToken = default);
}
