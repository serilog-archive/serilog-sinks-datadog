# Serilog.Sinks.Datadog

[![Build status](https://ci.appveyor.com/api/projects/status/btet68pac930p6p6/branch/master?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-datadog/branch/master)

Sends log events using Datadog.

**Package** - [Serilog.Sinks.Datadog](http://nuget.org/packages/serilog.sinks.datadog)
| **Platforms** - .NET 4.5.1, netstandard1.3, netstandard2.0

```csharp
var config = new DatadogConfiguration()
    .WithWithStatsdServer("127.0.0.1", 8125)
    .WithHostname("my-server")
    .WithTags("tag1", "tag2");

var log = new LoggerConfiguration()
    .WriteTo.Datadog(config)
    .CreateLogger();
```
