#nullable enable

using BauToolKit.Application.Interfaces;
using BauToolKit.Application.Services;
using BauToolKit.Domain.Models;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BauToolKit.UnitTests.Application;

public class CustomerServiceTests
{
    private readonly ICustomerRepository _customerRepo = Substitute.For<ICustomerRepository>();
    private readonly CustomerService _sut;

    public CustomerServiceTests()
    {
        _sut = new CustomerService(_customerRepo);
    }

    [Fact]
    public async Task CreateAsync_WhenValidCustomer_PersistsAndReturnsCustomer()
    {
        // Arrange
        Customer customer = new()
        {
            Name = "Musterbau GmbH",
            Email = "kontakt@musterbau.de",
            City = "Hamburg"
        };

        _customerRepo.AddAsync(customer, Arg.Any<CancellationToken>()).Returns(customer);

        // Act
        Customer result = await _sut.CreateAsync(customer);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Musterbau GmbH");
        await _customerRepo.Received(1).AddAsync(customer, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_WhenNameIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        Customer customer = new()
        {
            Name = "  ",
            City = "Hamburg"
        };

        // Act
        Func<Task> act = async () => await _sut.CreateAsync(customer);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Kundenname darf nicht leer sein*");
        await _customerRepo.DidNotReceive().AddAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange
        Guid customerId = Guid.NewGuid();
        _customerRepo.DeleteAsync(customerId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        bool result = await _sut.DeleteAsync(customerId);

        // Assert
        result.Should().BeTrue();
        await _customerRepo.Received(1).DeleteAsync(customerId, Arg.Any<CancellationToken>());
    }
}
