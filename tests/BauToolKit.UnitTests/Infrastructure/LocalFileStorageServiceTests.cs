#nullable enable

using BauToolKit.Infrastructure.Storage;
using FluentAssertions;
using Xunit;

namespace BauToolKit.UnitTests.Infrastructure;

public class LocalFileStorageServiceTests
{
    [Fact]
    public async Task SaveGetAndDeleteFileAsync_RoundTripsContent()
    {
        string directory = CreateTemporaryDirectory();

        try
        {
            LocalFileStorageService storage = new(directory);
            byte[] content = [1, 2, 3, 4];
            using MemoryStream input = new(content);

            string path = await storage.SaveFileAsync("../photo.jpg", input);
            Path.GetDirectoryName(path).Should().Be(directory);
            Path.GetFileName(path).Should().EndWith("_photo.jpg");

            await using Stream? savedFile = await storage.GetFileAsync(path);
            savedFile.Should().NotBeNull();
            using MemoryStream output = new();
            await savedFile!.CopyToAsync(output);
            output.ToArray().Should().Equal(content);

            (await storage.DeleteFileAsync(path)).Should().BeTrue();
            (await storage.GetFileAsync(path)).Should().BeNull();
            (await storage.DeleteFileAsync(path)).Should().BeFalse();
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task GetAndDeleteFileAsync_WhenFileDoesNotExist_ReturnNullAndFalse()
    {
        string directory = CreateTemporaryDirectory();

        try
        {
            LocalFileStorageService storage = new(directory);
            string missingPath = Path.Combine(directory, "missing.jpg");

            (await storage.GetFileAsync(missingPath)).Should().BeNull();
            (await storage.DeleteFileAsync(missingPath)).Should().BeFalse();
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task FileOperations_WhenCancellationRequested_ThrowOperationCanceledException()
    {
        string directory = CreateTemporaryDirectory();

        try
        {
            LocalFileStorageService storage = new(directory);
            using CancellationTokenSource cancellation = new();
            cancellation.Cancel();
            using MemoryStream content = new([1, 2, 3]);

            Func<Task> save = () => storage.SaveFileAsync("photo.jpg", content, cancellation.Token);
            Func<Task> get = () => storage.GetFileAsync(Path.Combine(directory, "photo.jpg"), cancellation.Token);
            Func<Task> delete = () => storage.DeleteFileAsync(Path.Combine(directory, "photo.jpg"), cancellation.Token);

            await save.Should().ThrowAsync<OperationCanceledException>();
            await get.Should().ThrowAsync<OperationCanceledException>();
            await delete.Should().ThrowAsync<OperationCanceledException>();
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static string CreateTemporaryDirectory()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"BauToolKitTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        return directory;
    }
}
