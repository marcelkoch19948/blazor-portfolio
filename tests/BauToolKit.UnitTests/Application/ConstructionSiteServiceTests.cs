#nullable enable

using BauToolKit.Application.Interfaces;
using BauToolKit.Application.Services;
using BauToolKit.Domain.Enums;
using BauToolKit.Domain.Models;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BauToolKit.UnitTests.Application;

public class ConstructionSiteServiceTests
{
    private readonly IConstructionSiteRepository _siteRepo = Substitute.For<IConstructionSiteRepository>();
    private readonly ICustomerRepository _customerRepo = Substitute.For<ICustomerRepository>();
    private readonly ConstructionSiteService _sut;

    public ConstructionSiteServiceTests()
    {
        _sut = new ConstructionSiteService(_siteRepo, _customerRepo);
    }

    [Fact]
    public async Task CreateAsync_WhenCustomerExists_PersistsAndReturnsSite()
    {
        // Arrange
        Guid customerId = Guid.NewGuid();
        Customer customer = new() { Id = customerId, Name = "Familie Meier" };

        ConstructionSite site = new()
        {
            CustomerId = customerId,
            Title = "Dachausbau Meier",
            Street = "Hauptstr. 10",
            ZipCode = "10115",
            City = "Berlin"
        };

        _customerRepo.GetByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns(customer);
        _siteRepo.AddAsync(site, Arg.Any<CancellationToken>()).Returns(site);

        // Act
        ConstructionSite result = await _sut.CreateAsync(site);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Dachausbau Meier");
        await _siteRepo.Received(1).AddAsync(site, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_WhenCustomerDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        Guid customerId = Guid.NewGuid();
        ConstructionSite site = new()
        {
            CustomerId = customerId,
            Title = "Dachausbau Meier"
        };

        _customerRepo.GetByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns((Customer?)null);

        // Act
        Func<Task> act = async () => await _sut.CreateAsync(site);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*existiert nicht*");
        await _siteRepo.DidNotReceive().AddAsync(Arg.Any<ConstructionSite>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProgressAsync_UpdatesProgressAndPersists()
    {
        // Arrange
        Guid siteId = Guid.NewGuid();
        ConstructionSite site = new()
        {
            Id = siteId,
            Title = "Badsanierung",
            Status = ConstructionSiteStatus.Geplant
        };

        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(site);
        _siteRepo.UpdateAsync(site, Arg.Any<CancellationToken>()).Returns(site);

        // Act
        ConstructionSite result = await _sut.UpdateProgressAsync(siteId, 45);

        // Assert
        result.ProgressPercentage.Should().Be(45);
        result.Status.Should().Be(ConstructionSiteStatus.InArbeit);
        await _siteRepo.Received(1).UpdateAsync(site, Arg.Any<CancellationToken>());
    }
}
