using SchedulR.Scheduling.Helpers;

namespace SchedulR.Tests.UnitTests.Scheduling.Helpers;

public class DateTimeOffsetHelpersUnitTests
{
    [Fact]
    public void PreciseUpToSecond_RemovesMillisecondsAndTicks()
    {
        // Arrange
        var original = new DateTimeOffset(2024, 10, 24, 12, 30, 45, 123, TimeSpan.Zero); // 123 ms
        var expected = new DateTimeOffset(2024, 10, 24, 12, 30, 45, TimeSpan.Zero); // Should remove ms

        // Act
        var result = original.PreciseUpToSecond();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void PreciseUpToSecond_KeepsDateTimeUpToSecond()
    {
        // Arrange
        var original = new DateTimeOffset(2024, 10, 24, 14, 45, 59, TimeSpan.FromHours(1)); // No ms to trim
        var expected = new DateTimeOffset(2024, 10, 24, 14, 45, 59, TimeSpan.FromHours(1));

        // Act
        var result = original.PreciseUpToSecond();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void PreciseUpToSecond_DifferentTimeZonesAreHandledCorrectly()
    {
        // Arrange
        var original = new DateTimeOffset(2024, 10, 24, 9, 30, 15, 567, TimeSpan.FromHours(-5)); // With ms and offset
        var expected = new DateTimeOffset(2024, 10, 24, 9, 30, 15, TimeSpan.FromHours(-5)); // Only up to second

        // Act
        var result = original.PreciseUpToSecond();

        // Assert
        result.Should().Be(expected);
    }
}
