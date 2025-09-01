using PipelineSchedulR.Scheduling.Mutex;

namespace PipelineSchedulR.Tests.UnitTests.Scheduling.Mutex;

public class ExecutableMutexUnitTests
{
    [Fact]
    public void TryAcquire_ShouldReturnTrue_WhenLockIsAcquiredForFirstTime()
    {
        // Arrange
        var mutex = new ExecutableMutex();
        string key = "testKey";

        // Act
        bool result = mutex.TryAcquire(key);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void TryAcquire_ShouldReturnFalse_WhenLockIsAlreadyAcquiredForSameKey()
    {
        // Arrange
        var mutex = new ExecutableMutex();
        string key = "testKey";

        // Act
        mutex.TryAcquire(key);
        bool result = mutex.TryAcquire(key);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Release_ShouldAllowKeyToBeAcquiredAgain_AfterBeingReleased()
    {
        // Arrange
        var mutex = new ExecutableMutex();
        string key = "testKey";

        // Act
        mutex.TryAcquire(key);
        mutex.Release(key);
        bool result = mutex.TryAcquire(key);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Release_ShouldNotThrow_WhenKeyIsNotInCollection()
    {
        // Arrange
        var mutex = new ExecutableMutex();
        string key = "nonExistentKey";

        // Act
        Action action = () => mutex.Release(key);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void TryAcquire_ShouldWorkIndependently_ForDifferentKeys()
    {
        // Arrange
        var mutex = new ExecutableMutex();
        string key1 = "key1";
        string key2 = "key2";

        // Act
        bool result1 = mutex.TryAcquire(key1);
        bool result2 = mutex.TryAcquire(key2);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
    }

    [Fact]
    public void TryAcquire_ShouldBeThreadSafe_WhenCalledConcurrently()
    {
        // Arrange
        var mutex = new ExecutableMutex();
        string key = "sharedKey";
        bool result1 = false, result2 = false;

        // Act
        var thread1 = new Thread(() =>
        {
            result1 = mutex.TryAcquire(key);
        });

        var thread2 = new Thread(() =>
        {
            result2 = mutex.TryAcquire(key);
        });

        thread1.Start();
        thread2.Start();

        thread1.Join();
        thread2.Join();

        // Assert
        (result1 || result2).Should().BeTrue(); // One should succeed
        (result1 && result2).Should().BeFalse(); // Both should not succeed
    }

    [Fact]
    public void Release_ShouldBeThreadSafe_WhenCalledConcurrently()
    {
        // Arrange
        var mutex = new ExecutableMutex();
        string key = "sharedKey";
        mutex.TryAcquire(key);
        bool result1 = false, result2 = false;

        // Act
        var thread1 = new Thread(() =>
        {
            mutex.Release(key);
            result1 = mutex.TryAcquire(key);
        });

        var thread2 = new Thread(() =>
        {
            mutex.Release(key);
            result2 = mutex.TryAcquire(key);
        });

        thread1.Start();
        thread2.Start();

        thread1.Join();
        thread2.Join();

        // Assert
        (result1 || result2).Should().BeTrue();  // One of the threads should reacquire the lock
    }
}
