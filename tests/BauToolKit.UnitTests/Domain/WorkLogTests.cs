#nullable enable

using BauToolKit.Domain.Models;
using FluentAssertions;
using Xunit;

namespace BauToolKit.UnitTests.Domain;

public class WorkLogTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(24)]
    public void SetHours_WhenWithinBounds_SetsHours(int hours)
    {
        WorkLog workLog = new();

        workLog.SetHours(hours);

        workLog.Hours.Should().Be(hours);
    }

    [Fact]
    public void SetHours_WhenFractionalHoursAreWithinBounds_SetsHours()
    {
        WorkLog workLog = new();

        workLog.SetHours(7.5m);

        workLog.Hours.Should().Be(7.5m);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(25)]
    public void SetHours_WhenOutsideBounds_ThrowsArgumentOutOfRangeException(int hours)
    {
        WorkLog workLog = new();

        Action act = () => workLog.SetHours(hours);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SetHours_WhenFractionalHoursExceedLimit_ThrowsArgumentOutOfRangeException()
    {
        WorkLog workLog = new();

        Action act = () => workLog.SetHours(24.01m);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ApprovalMethods_UpdateApprovalState()
    {
        WorkLog workLog = new();

        workLog.IsApprovedByChef.Should().BeFalse();

        workLog.Approve();
        workLog.IsApprovedByChef.Should().BeTrue();

        workLog.Reject();
        workLog.IsApprovedByChef.Should().BeFalse();

        workLog.Approve();
        workLog.IsApprovedByChef.Should().BeTrue();
    }
}
