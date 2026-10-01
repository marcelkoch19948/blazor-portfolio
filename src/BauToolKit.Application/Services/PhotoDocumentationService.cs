#nullable enable

using BauToolKit.Application.DTOs;
using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Services;

public class PhotoDocumentationService(
    IPhotoDocumentationRepository photoRepository,
    IConstructionSiteRepository siteRepository,
    IFileStorageService fileStorageService) : IPhotoDocumentationService
{
    public async Task<IReadOnlyList<PhotoDocumentation>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        return await photoRepository.GetBySiteIdAsync(siteId, cancellationToken);
    }

    public async Task<PhotoDocumentation> AddPhotoAsync(PhotoUploadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            throw new ArgumentException("Dateiname darf nicht leer sein.", nameof(request));
        }

        ArgumentNullException.ThrowIfNull(request.Content, nameof(request.Content));

        ConstructionSite? site = await siteRepository.GetByIdAsync(request.ConstructionSiteId, cancellationToken);
        if (site is null)
        {
            throw new KeyNotFoundException($"Baustelle mit Id '{request.ConstructionSiteId}' wurde nicht gefunden.");
        }

        string storagePath = await fileStorageService.SaveFileAsync(request.FileName, request.Content, cancellationToken);

        var photo = new PhotoDocumentation
        {
            ConstructionSiteId = request.ConstructionSiteId,
            UploadedByUserId = request.UploadedByUserId,
            FileName = request.FileName,
            StoragePath = storagePath,
            Description = request.Description,
            CapturedAtUtc = DateTime.UtcNow
        };

        return await photoRepository.AddAsync(photo, cancellationToken);
    }

    public async Task<bool> DeletePhotoAsync(Guid photoId, CancellationToken cancellationToken = default)
    {
        PhotoDocumentation? photo = await photoRepository.GetByIdAsync(photoId, cancellationToken);
        if (photo is null)
        {
            return false;
        }

        await fileStorageService.DeleteFileAsync(photo.StoragePath, cancellationToken);
        return await photoRepository.DeleteAsync(photoId, cancellationToken);
    }
}
