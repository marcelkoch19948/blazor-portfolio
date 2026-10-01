#nullable enable

using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Interfaces;

public interface IPhotoDocumentationRepository
{
    Task<IReadOnlyList<PhotoDocumentation>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<PhotoDocumentation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PhotoDocumentation> AddAsync(PhotoDocumentation photo, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
