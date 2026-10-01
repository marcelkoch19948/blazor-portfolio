#nullable enable

using BauToolKit.Domain.Enums;
using BauToolKit.Domain.Models;
using FluentAssertions;
using Xunit;

namespace BauToolKit.UnitTests.Domain;

public class InvoiceTests
{
    [Fact]
    public void CalculateNetAndGrossTotal_ComputesCorrectAmounts()
    {
        // Arrange
        Invoice invoice = new()
        {
            InvoiceNumber = "RE-2026-001",
            CustomerId = Guid.NewGuid(),
            Type = InvoiceType.Schlussrechnung,
            TaxRatePercentage = 19m
        };

        invoice.AddPosition(new InvoicePosition
        {
            Description = "Bauleitung & Koordination",
            Quantity = 10,
            Unit = "Std",
            UnitPrice = 85.00m
        });

        invoice.AddPosition(new InvoicePosition
        {
            Description = "Zementmörtel 25kg",
            Quantity = 20,
            Unit = "Sack",
            UnitPrice = 7.50m
        });

        // Act
        decimal netTotal = invoice.CalculateNetTotal();
        decimal vatAmount = invoice.CalculateTaxAmount();
        decimal grossTotal = invoice.CalculateGrossTotal();

        // Assert
        // Net: (10 * 85) + (20 * 7.50) = 850 + 150 = 1000
        netTotal.Should().Be(1000.00m);
        // VAT: 1000 * 0.19 = 190
        vatAmount.Should().Be(190.00m);
        // Gross: 1000 + 190 = 1190
        grossTotal.Should().Be(1190.00m);
    }
}
