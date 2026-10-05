#nullable enable

using BauToolKit.Domain.Models;
using BauToolKit.Infrastructure.Persistence;
using FluentAssertions;
using Xunit;

namespace BauToolKit.UnitTests.Infrastructure;

public class InMemoryCustomerRepositoryTests
{
    [Fact]
    public async Task CustomerOperations_StoreUpdateSortAndDeleteCustomers()
    {
        InMemoryCustomerRepository repository = new();
        Customer second = new() { Name = "Zweite Firma" };
        Customer first = new() { Name = "Erste Firma" };

        await repository.AddAsync(second);
        await repository.AddAsync(first);

        (await repository.GetAllAsync()).Select(customer => customer.Name)
            .Should().Equal("Erste Firma", "Zweite Firma");
        (await repository.GetByIdAsync(first.Id)).Should().BeSameAs(first);

        first.Name = "Aktualisierte Firma";
        await repository.UpdateAsync(first);

        (await repository.GetByIdAsync(first.Id))!.Name.Should().Be("Aktualisierte Firma");
        (await repository.DeleteAsync(second.Id)).Should().BeTrue();
        (await repository.DeleteAsync(second.Id)).Should().BeFalse();
        (await repository.GetByIdAsync(second.Id)).Should().BeNull();
    }

    [Fact]
    public async Task CustomerOperations_WhenCancellationRequested_ThrowOperationCanceledException()
    {
        InMemoryCustomerRepository repository = new();
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        Func<Task> getAll = () => repository.GetAllAsync(cancellation.Token);
        Func<Task> getById = () => repository.GetByIdAsync(Guid.NewGuid(), cancellation.Token);
        Func<Task> add = () => repository.AddAsync(new Customer(), cancellation.Token);
        Func<Task> update = () => repository.UpdateAsync(new Customer(), cancellation.Token);
        Func<Task> delete = () => repository.DeleteAsync(Guid.NewGuid(), cancellation.Token);

        await getAll.Should().ThrowAsync<OperationCanceledException>();
        await getById.Should().ThrowAsync<OperationCanceledException>();
        await add.Should().ThrowAsync<OperationCanceledException>();
        await update.Should().ThrowAsync<OperationCanceledException>();
        await delete.Should().ThrowAsync<OperationCanceledException>();
    }
}
