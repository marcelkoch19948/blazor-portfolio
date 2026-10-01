#nullable enable

using BauToolKit.Domain.Enums;
using BauToolKit.Domain.Models;
using FluentAssertions;
using Xunit;

namespace BauToolKit.UnitTests.Domain;

public class MaterialBookingTests
{
    [Theory]
    [InlineData(BookingType.Verbrauch, 5, -5)]
    [InlineData(BookingType.Zugang, 10, 10)]
    [InlineData(BookingType.Rueckgabe, 3, 3)]
    public void CalculateStockDelta_ReturnsCorrectDelta(BookingType bookingType, decimal quantity, decimal expectedDelta)
    {
        // Arrange
        MaterialBooking booking = new()
        {
            Type = bookingType,
            Quantity = quantity
        };

        // Act
        decimal delta = booking.CalculateStockDelta();

        // Assert
        delta.Should().Be(expectedDelta);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void CalculateStockDelta_WhenQuantityZeroOrNegative_ThrowsInvalidOperationException(decimal invalidQuantity)
    {
        // Arrange
        MaterialBooking booking = new()
        {
            Type = BookingType.Verbrauch,
            Quantity = invalidQuantity
        };

        // Act
        Action act = () => booking.CalculateStockDelta();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}
