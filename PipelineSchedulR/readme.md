# PipelineSchedulR

PipelineSchedulR is a lightweight, fluent API for scheduling tasks and building execution pipelines. Inspired by the low-configuration approach of Coravel and the structured pipeline design of MediatR, PipelineSchedulR enables developers to configure recurring task pipelines effortlessly, while maintaining a clean and maintainable codebase.

Features
--------

- **Fluent API for Task Scheduling**: Schedule recurring jobs using a simple, readable syntax that requires minimal configuration.
- **Pipeline Support**: Customize task execution using pipelines, inspired by MediatR, to handle cross-cutting concerns such as logging, validation, and exception handling.
- **Low Configuration**: With PipelineSchedulR, you can avoid the boilerplate and complexity found in other scheduling libraries, allowing you to focus on your application logic.
- **Flexible and Extensible**: Easily extend and modify task pipelines to fit your specific use cases, without sacrificing simplicity.

## Inspiration

PipelineSchedulR is inspired by the fantastic [Coravel](https://github.com/jamesmh/coravel) package, which provides fluent registration and task scheduling with minimal configuration. While PipelineSchedulR takes cues from Coravel's elegant design, it introduces several new concepts:

- **Pipelines**: Allow actions to be taken based on the result of a job, such as short-circuiting or handling specific outcomes.
- **Fully Async**: The package is designed to fully support asynchronous operations for modern .NET applications.
- **Configurable Lifecycle**: Unlike Coravel, PipelineSchedulR allows the scheduler to be started and stopped dynamically, rather than being permanently tied to the underlying `IHostApplicationLifetime`.

We’re grateful to the Coravel community for paving the way for low-configuration task scheduling in .NET and aim to expand on these ideas with additional flexibility and features.


## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Examples](#examples)
- [Contributing](#contributing)
- [License](#license)

## Installation

You can install PipelineSchedulR via NuGet: https://www.nuget.org/packages/PipelineSchedulR

```bash
dotnet add package PipelineSchedulR
```

## Examples

Here’s a quick example to demonstrate how to set up a job and pipeline using PipelineSchedulR:
```cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PipelineSchedulR.Common.Registration;
using PipelineSchedulR.Common.Types;
using PipelineSchedulR.Interfaces;

var host = Host.CreateDefaultBuilder(args)
               .ConfigureServices((context, services) =>
               {
                   services.AddScoped<HelloWorldJob>()
                           .AddScoped<LoggingPipeline>()
                           .AddSchedulR((pipelineBuilder, _) =>
                           {
                               pipelineBuilder
                                    .Executable<HelloWorldJob>()
                                    .WithPipeline<LoggingPipeline>();
                           });
               })
               .Build();

host.Services.UseSchedulR(scheduler =>
{
    scheduler
        .Schedule<HelloWorldJob>()
        .EveryMinutes(1);
});



class HelloWorldJob : IExecutable
{
    public async Task<Result> ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Hello, world!");

        await Task.Delay(100, cancellationToken); // Simulate work

        return Result.Success();
    }
}

class LoggingPipeline : IPipeline
{
    public async Task<Result> ExecuteAsync(PipelineDelegate next, CancellationToken cancellationToken)
    {
        Console.WriteLine("Executing pipeline!");

        var result = await next(cancellationToken);

        Console.WriteLine("Execution result: {0}", result.IsSuccess);

        return result;
    }
}
```

### Explanation

1. **Configure Services**: In the `Host.CreateDefaultBuilder`, we configure `HelloWorldJob` and `LoggingPipeline` and register them with the PipelineSchedulR pipeline.
2. **Schedule a Job**: Use the `UsePipelineSchedulR` method to define when and how frequently `HelloWorldJob` should execute.
3. **Create a Job**: The `HelloWorldJob` class implements `IExecutable` and contains the task logic.
4. **Add a Pipeline**: The `LoggingPipeline` class demonstrates a custom pipeline that logs execution details before and after running the job.

For more examples and advanced usage, check out our [documentation](#).

## Future Work

We are actively working on enhancing PipelineSchedulR with new features to make it even more versatile. One of the key features in development is:

- **Job Persistence**: An implementation for job persistence using a local JSON file is on the way. This will allow scheduled jobs and their configurations to be saved and restored across application restarts, providing greater flexibility and reliability for long-running applications.

Stay tuned for updates!







