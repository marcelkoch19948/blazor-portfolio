#nullable enable

using BauToolKit.Application.Interfaces;
using BauToolKit.Application.Services;
using BauToolKit.Domain.Enums;
using BauToolKit.Domain.Models;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BauToolKit.UnitTests.Application;

public class SiteAssignmentServiceTests
{
    private readonly ISiteAssignmentRepository _assignmentRepo = Substitute.For<ISiteAssignmentRepository>();
    private readonly IConstructionSiteRepository _siteRepo = Substitute.For<IConstructionSiteRepository>();
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly SiteAssignmentService _sut;

    public SiteAssignmentServiceTests()
    {
        _sut = new SiteAssignmentService(_assignmentRepo, _userRepo, _siteRepo);
    }

    [Fact]
    public async Task AssignEmployeeAsync_WhenValidEmployeeAndSite_CreatesAssignment()
    {
        // Arrange
        Guid siteId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Guid chefId = Guid.NewGuid();

        ConstructionSite site = new() { Id = siteId, Title = "Baustelle Nord" };
        User user = new() { Id = userId, FirstName = "Anna", LastName = "Mitarbeiterin", Role = UserRole.Mitarbeiter };

        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(site);
        _userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _assignmentRepo.GetBySiteIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(new List<SiteAssignment>());
        _assignmentRepo.AddAsync(Arg.Any<SiteAssignment>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<SiteAssignment>());

        // Act
        SiteAssignment result = await _sut.AssignEmployeeAsync(siteId, userId, chefId, "Hauptansprechpartner");

        // Assert
        result.Should().NotBeNull();
        result.ConstructionSiteId.Should().Be(siteId);
        result.UserId.Should().Be(userId);
        result.AssignedByUserId.Should().Be(chefId);
        result.IsActive.Should().BeTrue();
        result.Notes.Should().Be("Hauptansprechpartner");

        await _assignmentRepo.Received(1).AddAsync(Arg.Any<SiteAssignment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AssignEmployeeAsync_WhenAlreadyActivelyAssigned_ReturnsExistingAssignmentIdempotently()
    {
        // Arrange
        Guid siteId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        ConstructionSite site = new() { Id = siteId, Title = "Baustelle Nord" };
        User user = new() { Id = userId, FirstName = "Anna", LastName = "Mitarbeiterin", Role = UserRole.Mitarbeiter };

        SiteAssignment existing = new()
        {
            ConstructionSiteId = siteId,
            UserId = userId,
            IsActive = true
        };

        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(site);
        _userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _assignmentRepo.GetAsync(siteId, userId, Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(existing);

        // Act
        SiteAssignment result = await _sut.AssignEmployeeAsync(siteId, userId, null);

        // Assert
        result.Should().Be(existing);
        result.IsActive.Should().BeTrue();
        await _assignmentRepo.DidNotReceive().AddAsync(Arg.Any<SiteAssignment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AssignEmployeeAsync_WhenInactiveAssignmentExists_ReactivatesAssignment()
    {
        // Arrange
        Guid siteId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        ConstructionSite site = new() { Id = siteId, Title = "Baustelle Süd" };
        User user = new() { Id = userId, FirstName = "Max", LastName = "Mitarbeiter", Role = UserRole.Mitarbeiter };

        SiteAssignment existingInactive = new()
        {
            ConstructionSiteId = siteId,
            UserId = userId,
            IsActive = false,
            Notes = "Alte Zuweisung"
        };

        _siteRepo.GetByIdAsync(siteId, Arg.Any<CancellationToken>()).Returns(site);
        _userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _assignmentRepo.GetAsync(siteId, userId, Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(existingInactive);
        _assignmentRepo.UpdateAsync(Arg.Any<SiteAssignment>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<SiteAssignment>());

        // Act
        SiteAssignment result = await _sut.AssignEmployeeAsync(siteId, userId, null, "Reaktiviert");

        // Assert
        result.IsActive.Should().BeTrue();
        result.Notes.Should().Be("Reaktiviert");
        await _assignmentRepo.Received(1).UpdateAsync(existingInactive, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeactivateAssignmentAsync_WhenFound_MarksInactive()
    {
        // Arrange
        Guid assignmentId = Guid.NewGuid();
        SiteAssignment assignment = new()
        {
            Id = assignmentId,
            ConstructionSiteId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            IsActive = true
        };

        _assignmentRepo.GetByIdAsync(assignmentId, Arg.Any<CancellationToken>()).Returns(assignment);
        _assignmentRepo.UpdateAsync(assignment, Arg.Any<CancellationToken>()).Returns(assignment);

        // Act
        bool result = await _sut.DeactivateAssignmentAsync(assignmentId);

        // Assert
        result.Should().BeTrue();
        assignment.IsActive.Should().BeFalse();
        await _assignmentRepo.Received(1).UpdateAsync(assignment, Arg.Any<CancellationToken>());
    }
}
