# PipelineSchedulR

PipelineSchedulR is a lightweight and extensible API for scheduling recurring tasks and building execution pipelines in modern .NET applications. It offers a fluent configuration approach, enabling developers to set up background jobs and pipelines with minimal boilerplate and maximum clarity.

## Features

- **Intuitive Fluent API:** Easily schedule recurring jobs using clear, expressive syntax.
- **Configurable Execution Pipelines:** Compose pipelines to handle cross-cutting concerns such as logging, validation, and exception handling.
- **Low Configuration Overhead:** Focus on application logic without the complexities common in other scheduling solutions.
- **Fully Asynchronous:** Designed for modern .NET, all pipeline and job executions are inherently async-friendly.
- **Flexible Lifecycle:** Dynamically start or stop scheduling as needed, providing greater control over execution.
- **Extensible:** Adapt or extend to meet your application's unique requirements.

## Inspiration

PipelineSchedulR builds on best practices from established libraries, streamlining task scheduling while introducing new concepts:

- **Composable Pipelines:** Chain actions and custom logic before or after job execution, and even influence execution flow.
- **Modern & Flexible:** Supports dynamic lifecycle management and async operations throughout.

We gratefully acknowledge the foundations provided by open-source projects such as [Coravel](https://github.com/jamesmh/coravel), while aiming to push flexibility and extensibility further for diverse .NET scenarios.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Examples](#examples)
- [License](#license)

## Installation

Install via NuGet:

```bash
dotnet add package PipelineSchedulR
```

## Usage Example

Below is a basic example illustrating how to set up a job and assign it to a pipeline:

```csharp
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

// Implementation of a job
class HelloWorldJob : IExecutable
{
    public async Task<Result> ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Hello, world!");
        await Task.Delay(100, cancellationToken);
        return Result.Success();
    }
}

// Example pipeline for custom logic
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

1. **Service Registration:** Register jobs and pipelines using the provided fluent methods.
2. **Job Scheduling:** Specify frequency and execution conditions through expressive methods.
3. **Custom Logic:** Implement jobs and pipelines to encapsulate your business logic and cross-cutting concerns.

For further examples and advanced scenarios, please refer to the [documentation](#).

## Future Work

Planned enhancements include:

- **Job Persistence:** Support for persisting job configurations (e.g., to a JSON file), enabling stateful scheduling across application restarts.

Stay tuned for updates as new features are introduced!

## License

This project is licensed under the MIT License.