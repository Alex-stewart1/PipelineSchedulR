using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using PipelineSchedulR.Scheduling;
using PipelineSchedulR.Scheduling.Configuration;
using PipelineSchedulR.Tests.Mocks.Executable;

namespace PipelineSchedulR.Tests.UnitTests.Scheduling;

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

    public class Schedule
    {
        [Fact]
        public void Schedule_NewJob_ShouldSuccessfullyAddJob()
        {
            // Arrange
            var scheduler = new Scheduler(Substitute.For<IServiceScopeFactory>(), new SchedulerOptions());
            
            // Act
            scheduler.Schedule<ExecutableMock1>();
            
            // Assert
            scheduler.ScheduledJobs.Should().HaveCount(1);
        }
        
        [Fact]
        public void Schedule_DuplicateJob_ShouldThrow()
        {
            // Arrange
            var scheduler = new Scheduler(Substitute.For<IServiceScopeFactory>(), new SchedulerOptions());
            scheduler.Schedule<ExecutableMock1>();
            
            // Act
            var act = () => scheduler.Schedule<ExecutableMock1>();
            
            // Assert
            act.Should().Throw<InvalidOperationException>();
            scheduler.ScheduledJobs.Should().HaveCount(1);
        }
    }

    public class Deschedule
    {
        [Fact]
        public void Deschedule_ExistingJob_ShouldSuccessfullyRemoveJob()
        {
            // Arrange
            var scheduler = new Scheduler(Substitute.For<IServiceScopeFactory>(), new SchedulerOptions());
            scheduler.Schedule<ExecutableMock1>();
            
            // Act
            scheduler.Deschedule<ExecutableMock1>();
            
            // Assert
            scheduler.ScheduledJobs.Should().HaveCount(0);
        }
        
        [Fact]
        public void Deschedule_NonExistingJob_ShouldNotThrow()
        {
            // Arrange
            var scheduler = new Scheduler(Substitute.For<IServiceScopeFactory>(), new SchedulerOptions());
            
            // Act
            var act = () => scheduler.Deschedule<ExecutableMock1>();
            
            // Assert
            act.Should().NotThrow();
            scheduler.ScheduledJobs.Should().HaveCount(0);
        }
    }
}
