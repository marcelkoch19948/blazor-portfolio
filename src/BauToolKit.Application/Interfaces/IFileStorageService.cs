#nullable enable

namespace BauToolKit.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(string fileName, Stream content, CancellationToken cancellationToken = default);
    Task<Stream?> GetFileAsync(string storagePath, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string storagePath, CancellationToken cancellationToken = default);
}
