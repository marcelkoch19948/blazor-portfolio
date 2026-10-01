#nullable enable

using BauToolKit.Domain.Enums;
using BauToolKit.Domain.Models;
using FluentAssertions;
using Xunit;

namespace BauToolKit.UnitTests.Domain;

public class ConstructionSiteTests
{
    [Fact]
    public void UpdateProgress_WhenPositivePercentage_UpdatesStatusToInArbeit()
    {
        // Arrange
        ConstructionSite site = new()
        {
            Title = "Neubau EFH Müller",
            Status = ConstructionSiteStatus.Geplant
        };

        // Act
        site.UpdateProgress(25);

        // Assert
        site.ProgressPercentage.Should().Be(25);
        site.Status.Should().Be(ConstructionSiteStatus.InArbeit);
    }

    [Fact]
    public void UpdateProgress_WhenReaching100Percent_UpdatesStatusToAbgeschlossen()
    {
        // Arrange
        ConstructionSite site = new()
        {
            Title = "Dachsanierung Schmidt",
            Status = ConstructionSiteStatus.InArbeit
        };

        // Act
        site.UpdateProgress(100);

        // Assert
        site.ProgressPercentage.Should().Be(100);
        site.Status.Should().Be(ConstructionSiteStatus.Abgeschlossen);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void UpdateProgress_WhenInvalidPercentage_ThrowsArgumentOutOfRangeException(int invalidPercentage)
    {
        // Arrange
        ConstructionSite site = new() { Title = "Test Baustelle" };

        // Act
        Action act = () => site.UpdateProgress(invalidPercentage);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ChangeStatus_ToAbgeschlossen_SetsProgressTo100Percent()
    {
        // Arrange
        ConstructionSite site = new()
        {
            Title = "Umbau Lagerhalle",
            Status = ConstructionSiteStatus.InArbeit
        };
        site.UpdateProgress(50);

        // Act
        site.ChangeStatus(ConstructionSiteStatus.Abgeschlossen);

        // Assert
        site.Status.Should().Be(ConstructionSiteStatus.Abgeschlossen);
        site.ProgressPercentage.Should().Be(100);
    }
}
