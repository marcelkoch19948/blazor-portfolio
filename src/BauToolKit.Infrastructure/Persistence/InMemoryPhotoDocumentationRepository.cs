#nullable enable

using System.Collections.Concurrent;
using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Infrastructure.Persistence;

public class InMemoryPhotoDocumentationRepository : IPhotoDocumentationRepository
{
    private readonly ConcurrentDictionary<Guid, PhotoDocumentation> _photos = new();

    public Task<IReadOnlyList<PhotoDocumentation>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<PhotoDocumentation> list = _photos.Values
            .Where(p => p.ConstructionSiteId == siteId)
            .OrderByDescending(p => p.CapturedAtUtc)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<PhotoDocumentation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _photos.TryGetValue(id, out PhotoDocumentation? photo);
        return Task.FromResult(photo);
    }

    public Task<PhotoDocumentation> AddAsync(PhotoDocumentation photo, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(photo);
        _photos[photo.Id] = photo;
        return Task.FromResult(photo);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        bool removed = _photos.TryRemove(id, out _);
        return Task.FromResult(removed);
    }
}
