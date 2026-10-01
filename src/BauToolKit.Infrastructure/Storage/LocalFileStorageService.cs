#nullable enable

using BauToolKit.Application.Interfaces;

namespace BauToolKit.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _baseDirectory;

    public LocalFileStorageService(string? baseDirectory = null)
    {
        _baseDirectory = baseDirectory ?? Path.Combine(Path.GetTempPath(), "BauToolKit_Uploads");
        if (!Directory.Exists(_baseDirectory))
        {
            Directory.CreateDirectory(_baseDirectory);
        }
    }

    public async Task<string> SaveFileAsync(string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileName);
        ArgumentNullException.ThrowIfNull(content);

        string safeFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
        string targetPath = Path.Combine(_baseDirectory, safeFileName);

        FileStream fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await using (fileStream.ConfigureAwait(false))
        {
            await content.CopyToAsync(fileStream, cancellationToken);
        }

        return targetPath;
    }

    public Task<Stream?> GetFileAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!File.Exists(storagePath))
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = new FileStream(storagePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        return Task.FromResult<Stream?>(stream);
    }

    public Task<bool> DeleteFileAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!File.Exists(storagePath))
        {
            return Task.FromResult(false);
        }

        File.Delete(storagePath);
        return Task.FromResult(true);
    }
}
