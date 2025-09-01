using Microsoft.Extensions.DependencyInjection;
using PipelineSchedulR.Common.Helpers;
using PipelineSchedulR.Common.Registration;
using PipelineSchedulR.Interfaces;
using PipelineSchedulR.Tests.Mocks.Executable;
using PipelineSchedulR.Tests.Mocks.Pipeline;

namespace PipelineSchedulR.Tests.UnitTests.Common.Registration;

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
                     .Executable<ExecutableMock1>();
            });

            // Assert
            var serviceDescriptor = services.FirstOrDefault(x => (string?)x.ServiceKey == KeyedServiceHelper.GetExecutableKey(typeof(ExecutableMock1)) &&
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
                     .Executable<ExecutableMock1>()
                     .WithPipeline<PipelineMock1>();
            });

            // Assert
            var executableServiceDescriptor = services.FirstOrDefault(x => (string?)x.ServiceKey == KeyedServiceHelper.GetExecutableKey(typeof(ExecutableMock1)) &&
                                                                                    x.ServiceType == typeof(IExecutable));

            var pipelineServiceDescriptor = services.FirstOrDefault(x => (string?)x.ServiceKey == KeyedServiceHelper.GetExecutableKey(typeof(ExecutableMock1)) &&
                                                                                  x.ServiceType == typeof(IPipeline));

            executableServiceDescriptor.Should().NotBeNull();
            pipelineServiceDescriptor.Should().NotBeNull();
        }
    }
}