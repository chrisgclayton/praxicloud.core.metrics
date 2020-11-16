# PraxiCloud Core Metrics
PraxiCloud Libraries are a set of common utilities and tools for general software development that simplify common development efforts for software development. The core metrics library contains a basic provider based metrics framework modeled with a factory architecture.



# Installing via NuGet

Install-Package PraxiCloud-Core-Metrics



# Key Vault



## Key Types and Interfaces

|Class| Description | Notes |
| ------------- | ------------- | ------------- |
|**MetricFactory**|This is the main type to create for using the framework. The MetricFactory maintains a list of providers that the application registers with them and sets up counters etc. to write to all of them when created from the factory.<br />***AddProvider*** adds a metric provider to the factory. Sometimes this method is hidden from the user through extension methods created by the specific provider.<br />***CreateCounter*** creates a counter metric that is always increasing, never decreasing in value. Increment by one or custom values is supported.<br />***CreateGauge*** creates a gauge metric that can increase and decrease in value. Features are included to support tracking number of instances executing at a time.<br />***CreatePulse*** creates a pulse metric used to track the occurrence of a specific event.<br />***CreateSummary*** creates a summary metric that samples observed values over a period of time for tracking values such as 90th percentile.| This is the starting point that most metric clients being creating metric instances from. |
|**Counter**|A counter that increments in values and never decreases, only restarting when it is recreated.<br />***Increment*** increases the counter by one.<br />***IncrementBy*** increases the counter by a specified value.<br />***SetTo*** sets the counter to a specific number.| Use in the case of metrics that increase without ever decreasing. |
|**Gauge**|A gauge that increments in values and can decrease.<br />***Increment*** increases the gauge by one.<br />***IncrementBy*** increases the gauge by a specified value.<br />***Decrease*** decreases the gauge by one.<br />***DecreaseBy*** decreases the gauge by a specified value.<br />***SetTo*** sets the gauge to a specified value.<br />***TrackExecution*** tracks the number of items in a block, increasing when called and decreasing when the returned object is disposed of.| Use in the case of varying value metrics including ones that track the number of items executing a block of code. |
|**Pulse**|A metric that tracks the occurrence of a known event.<br />***Observe*** records the occurrence of a known event.| Use to track the occurrences of a known event. |
|**Summary**|Summaries a value over time, for use on aggregates, such as tracking the timing over time of a method.<br />***Observe*** records a value that has been observed.<br />***Time*** records the time it takes to execute a block of code, starting the timer when invoked and completing when the object returned is disposed of.| Use in the case of tracking timing of method calls over time, or values that are best used in aggregate. |
|**ConsoleMetricsProvider**|A provider that writes data to the console (standard output).|  |
|**DebugMetricsProvider**|A provider that writes data to the debug stream.|  |
|**TraceMetricsProvider**|A provider that writes data to the trace stream.|  |
|**CallbackMeticsProvider**|A provider that uses method callbacks when writes are being performed.|  |

## Sample Usage

### Add Providers to the Metric Factory

```csharp
using (var factory = new MetricFactory())
{
    factory.AddProvider(new MyCustomProvder());
    factory.AddDebug("debug", 1, true);
    factory.AddTrace("trace", 1, true);
    factory.AddConsole("console", 1, true);
    
    var summary = factory.CreateSummary("Metric1", "Test metric for #1", 5, true, new string[] { "label1", "label2" });

    for (var index = 0; index < 1500; index++)
    {
        summary.Observe(_doubleValues[index % _doubleValues.Length]);
        if (index < 1499) Task.Delay(10).GetAwaiter().GetResult();
    }
}
```

### Create a Counter and Increment the Value

```csharp
var counter = factory.CreateCounter("MyCounter", "A counter used to track my metrics", true, new string[] { "label1", "label2" });

for (var index = 0; index < 500; index++)
{
    counter.Increment();
    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
}
```

### Create a Gauge and Increment the Value

```csharp
var gauge = factory.CreateGauge("MyGauge", "A gauge used to track my metrics", true, new string[] { "label1", "label2" });

for (var index = 0; index < 500; index++)
{                   
    gauge.Increment();
    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
}
```

### Create a Gauge and Track Execution

```csharp
var gauge = factory.CreateGauge("MyGauge", "A gauge used to track my metrics", true, new string[] { "label1", "label2" });

for (var index = 0; index < 500; index++)
{                   
    using(gauge.TrackExecution())
    {
        // Do something
    }
}
```

### Create a Pulse and Record Observations

```csharp
var pulse = factory.CreatePulse("MyPulse", "A pulse used to track my metrics", true, new string[] { "label1", "label2" });

for (var index = 0; index < 500; index++)
{
    pulse.Observe();
    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
}
```

### Create a Summary and Record Observations

```csharp
var summary = factory.CreateSummary("MySummary", "A summary used to track my metrics", 5, true, new string[] { "label1", "label2" });

for (var index = 0; index < 1500; index++)
{
    summary.Observe(_doubleValues[index % _doubleValues.Length]);
    if (index < 1499) Task.Delay(10).GetAwaiter().GetResult();
}
```

### Create a Summary and Record Timings

```csharp
var summary = factory.CreateSummary("MySummary", "A summary used to track my metrics", 5, true, new string[] { "label1", "label2" });

for (var index = 0; index < 1500; index++)
{
    using (summary.Time())
    {
        // Perform action here
    }
}
```

## Additional Information

For additional information the Visual Studio generated documentation found [here](./documents/praxicloud.core.metrics/praxicloud.core.metrics.xml), can be viewed using your favorite documentation viewer.




