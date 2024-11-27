# SchedulR

SchedulR is a lightweight, fluent API for scheduling tasks and building execution pipelines. Inspired by the low-configuration approach of Coravel and the structured pipeline design of MediatR, SchedulR enables developers to configure recurring task pipelines effortlessly, while maintaining a clean and maintainable codebase.

Features
--------

- **Fluent API for Task Scheduling**: Schedule recurring jobs using a simple, readable syntax that requires minimal configuration.
- **Pipeline Support**: Customize task execution using pipelines, inspired by MediatR, to handle cross-cutting concerns such as logging, validation, and exception handling.
- **Low Configuration**: With SchedulR, you can avoid the boilerplate and complexity found in other scheduling libraries, allowing you to focus on your application logic.
- **Flexible and Extensible**: Easily extend and modify task pipelines to fit your specific use cases, without sacrificing simplicity.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Examples](#examples)
- [Contributing](#contributing)
- [License](#license)

## Installation

You can install SchedulR via NuGet:

```bash
dotnet add package SchedulR
```

## Examples

Here’s a quick example to demonstrate how to set up a job and pipeline using SchedulR:
```cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchedulR.Common.Registration;
using SchedulR.Common.Types;
using SchedulR.Interfaces;

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
    scheduler.Schedule<HelloWorldJob>()
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

1. **Configure Services**: In the `Host.CreateDefaultBuilder`, we configure `HelloWorldJob` and `LoggingPipeline` and register them with the SchedulR pipeline.
2. **Schedule a Job**: Use the `UseSchedulR` method to define when and how frequently `HelloWorldJob` should execute.
3. **Create a Job**: The `HelloWorldJob` class implements `IExecutable` and contains the task logic.
4. **Add a Pipeline**: The `LoggingPipeline` class demonstrates a custom pipeline that logs execution details before and after running the job.

For more examples and advanced usage, check out our [documentation](#).






