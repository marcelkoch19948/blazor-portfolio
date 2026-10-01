#nullable enable

using BauToolKit.Application.DTOs;
using BauToolKit.Application.Interfaces;
using BauToolKit.Application.Services;
using BauToolKit.Domain.Models;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BauToolKit.UnitTests.Application;

public class WorkLogServiceTests
{
    private readonly IWorkLogRepository _workLogRepo = Substitute.For<IWorkLogRepository>();
    private readonly IConstructionSiteRepository _siteRepo = Substitute.For<IConstructionSiteRepository>();
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly WorkLogService _sut;

    public WorkLogServiceTests()
    {
        _sut = new WorkLogService(_workLogRepo, _siteRepo, _userRepo);
    }

    [Fact]
    public async Task LogHoursAsync_WhenValidRequest_CreatesWorkLog()
    {
        // Arrange
        Guid siteId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        ConstructionSite site = new() { Id = siteId, Title = "Baustelle Süd" };
        User user = new() { Id = userId, FirstName = "Max", LastName = "Mustermann" };

        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(site);
        _userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _workLogRepo.AddAsync(Arg.Any<WorkLog>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<WorkLog>());

        WorkLogRequest request = new(
            userId,
            siteId,
            DateOnly.FromDateTime(DateTime.Today),
            7.5m,
            "Estrich verlegt und geglättet");

        // Act
        WorkLog result = await _sut.LogHoursAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Hours.Should().Be(7.5m);
        result.ActivityDescription.Should().Be("Estrich verlegt und geglättet");
        result.IsApprovedByChef.Should().BeFalse();
        await _workLogRepo.Received(1).AddAsync(Arg.Any<WorkLog>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LogHoursAsync_WhenHoursExceedLimit_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Guid siteId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        ConstructionSite site = new() { Id = siteId, Title = "Baustelle Süd" };
        User user = new() { Id = userId, FirstName = "Max", LastName = "Mustermann" };

        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(site);
        _userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        WorkLogRequest request = new(
            userId,
            siteId,
            DateOnly.FromDateTime(DateTime.Today),
            25m,
            "Mehr als 24 Stunden am Tag gearbeitet");

        // Act
        Func<Task> act = async () => await _sut.LogHoursAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        await _workLogRepo.DidNotReceive().AddAsync(Arg.Any<WorkLog>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ApproveHoursAsync_MarksLogAsApproved()
    {
        // Arrange
        Guid logId = Guid.NewGuid();
        WorkLog log = new()
        {
            Id = logId,
            UserId = Guid.NewGuid(),
            ConstructionSiteId = Guid.NewGuid()
        };
        log.SetHours(8m);

        _workLogRepo.GetByIdAsync(logId, Arg.Any<CancellationToken>()).Returns(log);
        _workLogRepo.UpdateAsync(log, Arg.Any<CancellationToken>()).Returns(log);

        // Act
        WorkLog result = await _sut.ApproveHoursAsync(logId);

        // Assert
        result.IsApprovedByChef.Should().BeTrue();
        await _workLogRepo.Received(1).UpdateAsync(log, Arg.Any<CancellationToken>());
    }
}
