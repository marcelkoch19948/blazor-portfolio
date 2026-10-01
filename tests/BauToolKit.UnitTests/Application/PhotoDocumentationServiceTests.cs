#nullable enable

using BauToolKit.Application.DTOs;
using BauToolKit.Application.Interfaces;
using BauToolKit.Application.Services;
using BauToolKit.Domain.Models;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BauToolKit.UnitTests.Application;

public class PhotoDocumentationServiceTests
{
    private readonly IPhotoDocumentationRepository _photoRepo = Substitute.For<IPhotoDocumentationRepository>();
    private readonly IConstructionSiteRepository _siteRepo = Substitute.For<IConstructionSiteRepository>();
    private readonly IFileStorageService _storageService = Substitute.For<IFileStorageService>();
    private readonly PhotoDocumentationService _sut;

    public PhotoDocumentationServiceTests()
    {
        _sut = new PhotoDocumentationService(_photoRepo, _siteRepo, _storageService);
    }

    [Fact]
    public async Task AddPhotoAsync_WhenSiteExistsAndInputValid_StoresFileAndAddsRecord()
    {
        // Arrange
        Guid siteId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        ConstructionSite site = new() { Id = siteId, Title = "Dachsanierung" };
        byte[] content = [1, 2, 3, 4, 5];

        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(site);
        _storageService.SaveFileAsync(
            Arg.Any<string>(),
            Arg.Any<Stream>(),
            Arg.Any<CancellationToken>()
        ).Returns("storage/photos/guid_dach.jpg");

        _photoRepo.AddAsync(Arg.Any<PhotoDocumentation>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<PhotoDocumentation>());

        using MemoryStream stream = new(content);
        PhotoUploadRequest request = new(
            ConstructionSiteId: siteId,
            UploadedByUserId: userId,
            FileName: "dach_ansicht.jpg",
            Content: stream,
            Description: "Aktuelle Ansicht Dach"
        );

        // Act
        PhotoDocumentation result = await _sut.AddPhotoAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.ConstructionSiteId.Should().Be(siteId);
        result.UploadedByUserId.Should().Be(userId);
        result.FileName.Should().Be("dach_ansicht.jpg");
        result.StoragePath.Should().Be("storage/photos/guid_dach.jpg");
        result.Description.Should().Be("Aktuelle Ansicht Dach");

        await _photoRepo.Received(1).AddAsync(Arg.Any<PhotoDocumentation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddPhotoAsync_WhenSiteDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        Guid siteId = Guid.NewGuid();
        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns((ConstructionSite?)null);

        using MemoryStream stream = new([1, 2, 3]);
        PhotoUploadRequest request = new(
            ConstructionSiteId: siteId,
            UploadedByUserId: Guid.NewGuid(),
            FileName: "test.jpg",
            Content: stream
        );

        // Act
        Func<Task> act = async () => await _sut.AddPhotoAsync(request);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*Baustelle mit Id '{siteId}' wurde nicht gefunden.*");
    }

    [Fact]
    public async Task AddPhotoAsync_WhenFileNameIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        Guid siteId = Guid.NewGuid();
        ConstructionSite site = new() { Id = siteId, Title = "Dachsanierung" };
        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(site);

        using MemoryStream stream = new([1, 2, 3]);
        PhotoUploadRequest request = new(
            ConstructionSiteId: siteId,
            UploadedByUserId: Guid.NewGuid(),
            FileName: "   ",
            Content: stream
        );

        // Act
        Func<Task> act = async () => await _sut.AddPhotoAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Dateiname darf nicht leer sein.*");
    }
}
