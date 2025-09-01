using PipelineSchedulR.Scheduling.Helpers;

namespace PipelineSchedulR.Tests.UnitTests.Scheduling.Helpers;

public class TickIntervalHelperTests
{
    [Fact]
    public void TicksToSeconds_ShouldConvertCorrectly_WhenTicksAreValid()
    {
        // Arrange
        long ticks = 5; // 5 ticks * 10 seconds per tick = 50 seconds
        long expectedSeconds = 50;

        // Act
        long result = TickIntervalHelper.TicksToSeconds(ticks);

        // Assert
        result.Should().Be(expectedSeconds);
    }

    [Fact]
    public void TicksToSeconds_ShouldThrowArgumentOutOfRangeException_WhenTicksExceedMaxTicksToSeconds()
    {
        // Arrange
        long invalidTicks = TickIntervalHelper.MaxTicksToSeconds + 1; // Exceeds limit

        // Act
        Action action = () => TickIntervalHelper.TicksToSeconds(invalidTicks);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
              .WithMessage("The ticks is too large to be converted to seconds.*");
    }

    [Fact]
    public void MinutesToTicks_ShouldConvertCorrectly_WhenMinutesAreValid()
    {
        // Arrange
        long minutes = 10; // 10 minutes * 6 ticks per minute = 60 ticks
        long expectedTicks = 60;

        // Act
        long result = TickIntervalHelper.MinutesToTicks(minutes);

        // Assert
        result.Should().Be(expectedTicks);
    }

    [Fact]
    public void MinutesToTicks_ShouldThrowArgumentOutOfRangeException_WhenMinutesExceedMaxMinutesToTicks()
    {
        // Arrange
        long invalidMinutes = TickIntervalHelper.MaxMinutesToTicks + 1; // Exceeds limit

        // Act
        Action action = () => TickIntervalHelper.MinutesToTicks(invalidMinutes);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
              .WithMessage("The minutes is too large to be converted to ticks.*");
    }

    [Fact]
    public void TicksToSeconds_ShouldReturnZero_WhenTicksAreZero()
    {
        // Arrange
        long ticks = 0;
        long expectedSeconds = 0;

        // Act
        long result = TickIntervalHelper.TicksToSeconds(ticks);

        // Assert
        result.Should().Be(expectedSeconds);
    }

    [Fact]
    public void MinutesToTicks_ShouldReturnZero_WhenMinutesAreZero()
    {
        // Arrange
        long minutes = 0;
        long expectedTicks = 0;

        // Act
        long result = TickIntervalHelper.MinutesToTicks(minutes);

        // Assert
        result.Should().Be(expectedTicks);
    }

    [Fact]
    public void TicksToSeconds_ShouldHandleMaxValidTickValue()
    {
        // Arrange
        long maxValidTicks = TickIntervalHelper.MaxTicksToSeconds;
        long expectedSeconds = maxValidTicks * 10; // max ticks * seconds per tick

        // Act
        long result = TickIntervalHelper.TicksToSeconds(maxValidTicks);

        // Assert
        result.Should().Be(expectedSeconds);
    }

    [Fact]
    public void MinutesToTicks_ShouldHandleMaxValidMinutesValue()
    {
        // Arrange
        long maxValidMinutes = TickIntervalHelper.MaxMinutesToTicks;
        long expectedTicks = maxValidMinutes * 6; // max minutes * ticks per minute

        // Act
        long result = TickIntervalHelper.MinutesToTicks(maxValidMinutes);

        // Assert
        result.Should().Be(expectedTicks);
    }
}

