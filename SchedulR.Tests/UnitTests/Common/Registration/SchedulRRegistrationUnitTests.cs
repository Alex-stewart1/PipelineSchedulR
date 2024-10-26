using Microsoft.Extensions.DependencyInjection;
using SchedulR.Common.Helpers;
using SchedulR.Common.Registration;
using SchedulR.Interfaces;
using SchedulR.Tests.Stubs.Pipeline;

namespace SchedulR.Tests.UnitTests.Common.Registration;

public class SchedulRRegistrationUnitTests
{
    public class AddSchedulR
    {
        [Fact]
        public void ExecutableCalled_ShouldAddExecutableAsKeyedService()
        {
            //Arrange
            var services = new ServiceCollection();

            // Act
            services.AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                     .Executable<ExecutableStub1>();
            });

            // Assert
            var serviceDescriptor = services.FirstOrDefault(x => (string?)x.ServiceKey == KeyedServiceHelper.GetExecutableKey(typeof(ExecutableStub1)) &&
                                                                          x.ServiceType == typeof(IExecutable));

            serviceDescriptor.Should().NotBeNull();
        }
        [Fact]
        public void WithPipelineCalled_ShouldAddPipelineAsKeyedService()
        {
            //Arrange
            var services = new ServiceCollection();

            // Act
            services.AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                     .Executable<ExecutableStub1>()
                     .WithPipeline<PipelineStub1>();
            });

            // Assert
            var executableServiceDescriptor = services.FirstOrDefault(x => (string?)x.ServiceKey == KeyedServiceHelper.GetExecutableKey(typeof(ExecutableStub1)) &&
                                                                                    x.ServiceType == typeof(IExecutable));

            var pipelineServiceDescriptor = services.FirstOrDefault(x => (string?)x.ServiceKey == KeyedServiceHelper.GetExecutableKey(typeof(ExecutableStub1)) &&
                                                                                  x.ServiceType == typeof(IPipeline));

            executableServiceDescriptor.Should().NotBeNull();
            pipelineServiceDescriptor.Should().NotBeNull();
        }
    }
}