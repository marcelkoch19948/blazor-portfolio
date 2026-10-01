#nullable enable

using BauToolKit.Application.DTOs;
using BauToolKit.Application.Interfaces;
using BauToolKit.Application.Services;
using BauToolKit.Domain.Enums;
using BauToolKit.Domain.Models;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BauToolKit.UnitTests.Application;

public class InventoryServiceTests
{
    private readonly IInventoryRepository _inventoryRepo = Substitute.For<IInventoryRepository>();
    private readonly IMaterialBookingRepository _bookingRepo = Substitute.For<IMaterialBookingRepository>();
    private readonly IConstructionSiteRepository _siteRepo = Substitute.For<IConstructionSiteRepository>();
    private readonly InventoryService _sut;

    public InventoryServiceTests()
    {
        _sut = new InventoryService(_inventoryRepo, _bookingRepo, _siteRepo);
    }

    [Fact]
    public async Task BookMaterialAsync_WhenOutgoingAndSufficientStock_DeductsStockAndPersistsBooking()
    {
        // Arrange
        Guid itemId = Guid.NewGuid();
        Guid siteId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        InventoryItem item = new()
        {
            Id = itemId,
            ItemNumber = "MAT-001",
            Name = "Kabelkanal 2m",
            Unit = "Stk"
        };
        item.AdjustStock(50m);

        ConstructionSite site = new() { Id = siteId, Title = "Baustelle A" };

        _inventoryRepo.GetByIdAsync(itemId, Arg.Any<CancellationToken>()).Returns(item);
        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(site);
        _inventoryRepo.UpdateAsync(item, Arg.Any<CancellationToken>()).Returns(item);
        _bookingRepo.AddAsync(Arg.Any<MaterialBooking>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<MaterialBooking>());

        MaterialBookingRequest request = new(itemId, siteId, userId, BookingType.Verbrauch, 10m, "Baustellenbedarf");

        // Act
        MaterialBooking result = await _sut.BookMaterialAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.InventoryItemId.Should().Be(itemId);
        result.ConstructionSiteId.Should().Be(siteId);
        result.Quantity.Should().Be(10m);
        item.CurrentStock.Should().Be(40m);

        await _inventoryRepo.Received(1).UpdateAsync(item, Arg.Any<CancellationToken>());
        await _bookingRepo.Received(1).AddAsync(Arg.Any<MaterialBooking>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BookMaterialAsync_WhenOutgoingAndInsufficientStock_ThrowsInvalidOperationException()
    {
        // Arrange
        Guid itemId = Guid.NewGuid();
        Guid siteId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        InventoryItem item = new()
        {
            Id = itemId,
            ItemNumber = "MAT-002",
            Name = "Spezialkleber",
            Unit = "Dose"
        };
        item.AdjustStock(3m);

        ConstructionSite site = new() { Id = siteId, Title = "Baustelle B" };

        _inventoryRepo.GetByIdAsync(itemId, Arg.Any<CancellationToken>()).Returns(item);
        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(site);

        MaterialBookingRequest request = new(itemId, siteId, userId, BookingType.Verbrauch, 5m, "Zu viel entnommen");

        // Act
        Func<Task> act = async () => await _sut.BookMaterialAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*nicht negativ werden*");
        await _bookingRepo.DidNotReceive().AddAsync(Arg.Any<MaterialBooking>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BookMaterialAsync_WhenIncomingStock_IncreasesStock()
    {
        // Arrange
        Guid itemId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        InventoryItem item = new()
        {
            Id = itemId,
            ItemNumber = "MAT-003",
            Name = "Schrauben 5x50",
            Unit = "Pack"
        };
        item.AdjustStock(100m);

        _inventoryRepo.GetByIdAsync(itemId, Arg.Any<CancellationToken>()).Returns(item);
        _inventoryRepo.UpdateAsync(item, Arg.Any<CancellationToken>()).Returns(item);
        _bookingRepo.AddAsync(Arg.Any<MaterialBooking>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<MaterialBooking>());

        MaterialBookingRequest request = new(itemId, null, userId, BookingType.Zugang, 25m, "Lieferung Großhandel");

        // Act
        MaterialBooking result = await _sut.BookMaterialAsync(request);

        // Assert
        result.Should().NotBeNull();
        item.CurrentStock.Should().Be(125m);
        await _inventoryRepo.Received(1).UpdateAsync(item, Arg.Any<CancellationToken>());
    }
}
