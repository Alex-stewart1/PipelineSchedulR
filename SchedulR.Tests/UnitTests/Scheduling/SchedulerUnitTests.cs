using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SchedulR.Scheduling;
using SchedulR.Scheduling.Configuration;

namespace SchedulR.Tests.UnitTests.Scheduling;

public class SchedulerUnitTests
{
    public class Start
    {
        private readonly Scheduler _scheduler;
        public Start()
        {
            _scheduler = new Scheduler(Substitute.For<IServiceScopeFactory>(), new SchedulerOptions());
        }
        [Fact]
        public void WhenSchedulerAlreadyStarted_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _scheduler.Start();

            // Act
            Action act = () => _scheduler.Start();

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("The scheduler has already started.");
        }

        [Fact]
        public void WhenSchedulerNotStarted_ShouldSetHasStartedToTrue()
        {
            // Act
            _scheduler.Start();

            // Assert
            _scheduler.HasStarted.Should().BeTrue();
        }

        [Fact]
        public void ShouldTriggerStartRequestedEvent()
        {
            // Arrange
            bool eventTriggered = false;
            _scheduler.StartRequested += (sender, args) => eventTriggered = true;

            // Act
            _scheduler.Start();

            // Assert
            eventTriggered.Should().BeTrue();
        }

    }

    public class StartAt
    {
        private readonly Scheduler _scheduler;
        public StartAt()
        {
            _scheduler = new Scheduler(Substitute.For<IServiceScopeFactory>(), new SchedulerOptions());
        }
        [Fact]
        public void Start_WhenSchedulerAlreadyStarted_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _scheduler.Start();

            // Act
            Action act = () => _scheduler.Start();

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("The scheduler has already started.");
        }

        [Fact]
        public void Start_WhenSchedulerNotStarted_ShouldSetHasStartedToTrue()
        {
            // Act
            _scheduler.Start();

            // Assert
            _scheduler.HasStarted.Should().BeTrue();
        }

        [Fact]
        public void Start_ShouldTriggerStartRequestedEvent()
        {
            // Arrange
            bool eventTriggered = false;
            _scheduler.StartRequested += (sender, args) => eventTriggered = true;

            // Act
            _scheduler.Start();

            // Assert
            eventTriggered.Should().BeTrue();
        }

        [Fact]
        public void StartAt_WhenSchedulerAlreadyStarted_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _scheduler.Start();

            // Act
            Action act = () => _scheduler.StartAt(DateTimeOffset.Now);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("The scheduler has already started.");
        }

        [Fact]
        public void StartAt_WhenSchedulerNotStarted_ShouldSetHasStartedToTrue()
        {
            // Act
            _scheduler.StartAt(DateTimeOffset.Now);

            // Assert
            _scheduler.HasStarted.Should().BeTrue();
        }
    }

}
