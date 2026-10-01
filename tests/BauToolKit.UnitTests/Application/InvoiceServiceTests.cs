#nullable enable

using BauToolKit.Application.Interfaces;
using BauToolKit.Application.Services;
using BauToolKit.Domain.Enums;
using BauToolKit.Domain.Models;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BauToolKit.UnitTests.Application;

public class InvoiceServiceTests
{
    private readonly IInvoiceRepository _invoiceRepo = Substitute.For<IInvoiceRepository>();
    private readonly IConstructionSiteRepository _siteRepo = Substitute.For<IConstructionSiteRepository>();
    private readonly ICustomerRepository _customerRepo = Substitute.For<ICustomerRepository>();
    private readonly InvoiceService _sut;

    public InvoiceServiceTests()
    {
        _sut = new InvoiceService(_invoiceRepo, _siteRepo, _customerRepo);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllInvoices()
    {
        // Arrange
        List<Invoice> invoices =
        [
            new() { InvoiceNumber = "RE-001" },
            new() { InvoiceNumber = "RE-002" }
        ];
        _invoiceRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(invoices);

        // Act
        IReadOnlyList<Invoice> result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(i => i.InvoiceNumber == "RE-001");
    }

    [Fact]
    public async Task CreateInvoiceAsync_WhenValid_AddsAndReturnsInvoice()
    {
        // Arrange
        Guid siteId = Guid.NewGuid();
        Guid customerId = Guid.NewGuid();

        ConstructionSite site = new() { Id = siteId, Title = "Neubau" };
        Customer customer = new() { Id = customerId, Name = "Familie Meier" };

        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(site);
        _customerRepo.GetByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns(customer);
        _invoiceRepo.AddAsync(Arg.Any<Invoice>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Invoice>());

        Invoice invoice = new()
        {
            InvoiceNumber = "RE-2026-101",
            ConstructionSiteId = siteId,
            CustomerId = customerId,
            Type = InvoiceType.Abschlagsrechnung
        };
        invoice.AddPosition(new InvoicePosition
        {
            Description = "1. Bauabschnitt Rohbau",
            Quantity = 1m,
            Unit = "pauschal",
            UnitPrice = 5000m
        });

        // Act
        Invoice result = await _sut.CreateInvoiceAsync(invoice);

        // Assert
        result.Should().NotBeNull();
        result.InvoiceNumber.Should().Be("RE-2026-101");
        result.Positions.Should().HaveCount(1);
        await _invoiceRepo.Received(1).AddAsync(invoice, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateInvoiceAsync_WhenInvoiceNumberEmpty_ThrowsArgumentException()
    {
        // Arrange
        Invoice invoice = new()
        {
            InvoiceNumber = "   ",
            ConstructionSiteId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid()
        };

        // Act
        Func<Task> act = async () => await _sut.CreateInvoiceAsync(invoice);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Rechnungsnummer darf nicht leer sein.*");
    }

    [Fact]
    public async Task CreateInvoiceAsync_WhenSiteNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        Guid siteId = Guid.NewGuid();
        Invoice invoice = new()
        {
            InvoiceNumber = "RE-001",
            ConstructionSiteId = siteId,
            CustomerId = Guid.NewGuid()
        };
        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns((ConstructionSite?)null);

        // Act
        Func<Task> act = async () => await _sut.CreateInvoiceAsync(invoice);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*Baustelle mit Id '{siteId}' wurde nicht gefunden.*");
    }

    [Fact]
    public async Task CreateInvoiceAsync_WhenCustomerNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        Guid siteId = Guid.NewGuid();
        Guid customerId = Guid.NewGuid();
        ConstructionSite site = new() { Id = siteId, Title = "Neubau" };

        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(site);
        _customerRepo.GetByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns((Customer?)null);

        Invoice invoice = new()
        {
            InvoiceNumber = "RE-001",
            ConstructionSiteId = siteId,
            CustomerId = customerId
        };

        // Act
        Func<Task> act = async () => await _sut.CreateInvoiceAsync(invoice);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*Kunde mit Id '{customerId}' wurde nicht gefunden.*");
    }

    [Fact]
    public async Task CreateInvoiceAsync_WhenNoPositions_ThrowsInvalidOperationException()
    {
        // Arrange
        Guid siteId = Guid.NewGuid();
        Guid customerId = Guid.NewGuid();
        ConstructionSite site = new() { Id = siteId, Title = "Neubau" };
        Customer customer = new() { Id = customerId, Name = "Familie Meier" };

        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(site);
        _customerRepo.GetByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns(customer);

        Invoice invoice = new()
        {
            InvoiceNumber = "RE-001",
            ConstructionSiteId = siteId,
            CustomerId = customerId
        };

        // Act
        Func<Task> act = async () => await _sut.CreateInvoiceAsync(invoice);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Eine Rechnung muss mindestens eine Position enthalten.*");
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenFound_UpdatesStatusAndReturns()
    {
        // Arrange
        Guid invoiceId = Guid.NewGuid();
        Invoice invoice = new()
        {
            Id = invoiceId,
            InvoiceNumber = "RE-001",
            Status = InvoiceStatus.Entwurf
        };

        _invoiceRepo.GetByIdAsync(invoiceId, Arg.Any<CancellationToken>()).Returns(invoice);
        _invoiceRepo.UpdateAsync(invoice, Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Invoice>());

        // Act
        Invoice result = await _sut.UpdateStatusAsync(invoiceId, InvoiceStatus.Gestellt);

        // Assert
        result.Status.Should().Be(InvoiceStatus.Gestellt);
        await _invoiceRepo.Received(1).UpdateAsync(invoice, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        Guid invoiceId = Guid.NewGuid();
        _invoiceRepo.GetByIdAsync(invoiceId, Arg.Any<CancellationToken>()).Returns((Invoice?)null);

        // Act
        Func<Task> act = async () => await _sut.UpdateStatusAsync(invoiceId, InvoiceStatus.Gestellt);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*Rechnung mit Id '{invoiceId}' wurde nicht gefunden.*");
    }
}
