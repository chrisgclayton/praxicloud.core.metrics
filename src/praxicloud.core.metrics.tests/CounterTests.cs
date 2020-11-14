// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.tests
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using praxicloud.core.metrics.callbackprovider;
    using praxicloud.core.metrics.consoleprovider;
    using praxicloud.core.metrics.debugprovider;
    using praxicloud.core.metrics.traceprovider;
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// A set of tests to validate the counters
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CounterTests
    {
        #region Simple Counter
        /// <summary>
        /// Counts up to 500
        /// </summary>
        [TestMethod]
        public void DelayingPublish()
        {
            var singleValues = new ConcurrentBag<SingleMetricHolder>();
            var summaryValues = new ConcurrentBag<SummaryMetricHolder>();

            using (var factory = new MetricFactory())
            {
                factory.AddCallback("callback1", 1, (object userState, string name, string[] labels, double? value, CancellationToken cancellationToken) =>
                {
                    singleValues.Add(new SingleMetricHolder { SampleTime = DateTime.UtcNow, Labels = labels, Name = name, UserState = userState, Value = value });
                    return Task.CompletedTask;
                },
                (object userState, string name, string[] labels, int? count, double? minimum, double? maximum, double? mean, double? standardDeviation, double? p50, double? p90, double? p95, double? p98, double? p99, CancellationToken cancellationToken) =>
                {
                    summaryValues.Add(new SummaryMetricHolder { SampleTime = DateTime.UtcNow, Labels = labels, Name = name, UserState = userState, Count = count, Minimum = minimum, Maximum = maximum, Mean = mean, StandardDeviation = standardDeviation, p50 = p50, p90 = p90, p95 = p95, p98 = p98, p99 = p99 });
                    return Task.CompletedTask;
                }, "test");

                factory.AddProvider("debug", new DebugMetricsProvider(1, true));
                factory.AddProvider("trace", new TraceMetricsProvider(1, true));
                factory.AddProvider("console", new ConsoleMetricsProvider(1, true));

                var counter = factory.CreateCounter("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });
                                
                Task.Delay(5000).GetAwaiter().GetResult();
            }

            Assert.IsTrue(summaryValues.Count == 0, "Single value count not expected");
            Assert.IsTrue(summaryValues.Count == 0, "Summary value count not expected");
        }

        /// <summary>
        /// Counts up to 500
        /// </summary>
        [TestMethod]
        public void SimpleCountIteration()
        {
            var singleValues = new ConcurrentBag<SingleMetricHolder>();
            var summaryValues = new ConcurrentBag<SummaryMetricHolder>();

            using (var factory = new MetricFactory())
            {
                factory.AddCallback("callback1", 1, (object userState, string name, string[] labels, double? value, CancellationToken cancellationToken) =>
                {
                    singleValues.Add(new SingleMetricHolder { SampleTime = DateTime.UtcNow, Labels = labels, Name = name, UserState = userState, Value = value });
                    return Task.CompletedTask;
                },
                (object userState, string name, string[] labels, int? count, double? minimum, double? maximum, double? mean, double? standardDeviation, double? p50, double? p90, double? p95, double? p98, double? p99, CancellationToken cancellationToken) =>
                {
                    summaryValues.Add(new SummaryMetricHolder { SampleTime = DateTime.UtcNow, Labels = labels, Name = name, UserState = userState, Count = count, Minimum = minimum, Maximum = maximum, Mean = mean, StandardDeviation = standardDeviation, p50 = p50, p90 = p90, p95 = p95, p98 = p98, p99 = p99 });
                    return Task.CompletedTask;
                }, "test");

                var counter = factory.CreateCounter("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    counter.Increment();
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = singleValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();

                Assert.IsTrue(itemList.Length >= 6, $"Single value count within groupings not within expected tolerance (value { itemList.Length })");
                Assert.IsTrue((itemList.Max(item => item.Value ?? 0)) >= 500, "Maximum value count not expected");
            }

            Assert.IsTrue(summaryValues.Count == 0, "Summary value count not expected");
        }

        /// <summary>
        /// Counts to 5000 by 10s
        /// </summary>
        [TestMethod]
        public void SimpleCountIterationBy()
        {
            var singleValues = new ConcurrentBag<SingleMetricHolder>();
            var summaryValues = new ConcurrentBag<SummaryMetricHolder>();

            using (var factory = new MetricFactory())
            {
                factory.AddCallback("callback1", 1, (object userState, string name, string[] labels, double? value, CancellationToken cancellationToken) =>
                {
                    singleValues.Add(new SingleMetricHolder { SampleTime = DateTime.UtcNow, Labels = labels, Name = name, UserState = userState, Value = value });
                    return Task.CompletedTask;
                },
                (object userState, string name, string[] labels, int? count, double? minimum, double? maximum, double? mean, double? standardDeviation, double? p50, double? p90, double? p95, double? p98, double? p99, CancellationToken cancellationToken) =>
                {
                    summaryValues.Add(new SummaryMetricHolder { SampleTime = DateTime.UtcNow, Labels = labels, Name = name, UserState = userState, Count = count, Minimum = minimum, Maximum = maximum, Mean = mean, StandardDeviation = standardDeviation, p50 = p50, p90 = p90, p95 = p95, p98 = p98, p99 = p99 });
                    return Task.CompletedTask;
                }, "test");

                var counter = factory.CreateCounter("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    counter.IncrementBy(10);
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = singleValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();

                Assert.IsTrue(itemList.Length >= 6, $"Single value count within groupings not within expected tolerance (value { itemList.Length })");
                Assert.IsTrue(itemList.Max(item => item.Value.Value) == 5000, "Maximum value count not expected");
            }

            Assert.IsTrue(summaryValues.Count == 0, "Summary value count not expected");
        }

        /// <summary>
        /// Counts up to 500
        /// </summary>
        [TestMethod]
        public void SimpleCountIterationMultiProvider()
        {
            var singleValues = new ConcurrentBag<SingleMetricHolder>();
            var summaryValues = new ConcurrentBag<SummaryMetricHolder>();

            using (var factory = new MetricFactory())
            {
                factory.AddCallback("callback1", 1, (object userState, string name, string[] labels, double? value, CancellationToken cancellationToken) =>
                {
                    singleValues.Add(new SingleMetricHolder { SampleTime = DateTime.UtcNow, Labels = labels, Name = name, UserState = userState, Value = value });
                    return Task.CompletedTask;
                },
                (object userState, string name, string[] labels, int? count, double? minimum, double? maximum, double? mean, double? standardDeviation, double? p50, double? p90, double? p95, double? p98, double? p99, CancellationToken cancellationToken) =>
                {
                    summaryValues.Add(new SummaryMetricHolder { SampleTime = DateTime.UtcNow, Labels = labels, Name = name, UserState = userState, Count = count, Minimum = minimum, Maximum = maximum, Mean = mean, StandardDeviation = standardDeviation, p50 = p50, p90 = p90, p95 = p95, p98 = p98, p99 = p99 });
                    return Task.CompletedTask;
                }, "test");

                factory.AddProvider("debug", new DebugMetricsProvider(1, true));
                factory.AddProvider("trace", new TraceMetricsProvider(1, true));
                factory.AddProvider("console", new ConsoleMetricsProvider(1, true));

                var counter = factory.CreateCounter("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    counter.Increment();
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = singleValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();

                Assert.IsTrue(itemList.Length >= 6, $"Single value count within groupings not within expected tolerance (value { itemList.Length })");
                Assert.IsTrue((itemList.Max(item => item.Value ?? 0)) >= 500, "Maximum value count not expected");
            }

            Assert.IsTrue(summaryValues.Count == 0, "Summary value count not expected");
        }

        /// <summary>
        /// Counts to 5000 by 10s
        /// </summary>
        [TestMethod]
        public void SimpleCountIterationByMultiProvider()
        {
            var singleValues = new ConcurrentBag<SingleMetricHolder>();
            var summaryValues = new ConcurrentBag<SummaryMetricHolder>();

            using (var factory = new MetricFactory())
            {
                factory.AddCallback("callback1", 1, (object userState, string name, string[] labels, double? value, CancellationToken cancellationToken) =>
                {
                    singleValues.Add(new SingleMetricHolder { SampleTime = DateTime.UtcNow, Labels = labels, Name = name, UserState = userState, Value = value });
                    return Task.CompletedTask;
                },
                (object userState, string name, string[] labels, int? count, double? minimum, double? maximum, double? mean, double? standardDeviation, double? p50, double? p90, double? p95, double? p98, double? p99, CancellationToken cancellationToken) =>
                {
                    summaryValues.Add(new SummaryMetricHolder { SampleTime = DateTime.UtcNow, Labels = labels, Name = name, UserState = userState, Count = count, Minimum = minimum, Maximum = maximum, Mean = mean, StandardDeviation = standardDeviation, p50 = p50, p90 = p90, p95 = p95, p98 = p98, p99 = p99 });
                    return Task.CompletedTask;
                }, "test");

                factory.AddProvider("debug", new DebugMetricsProvider(1, true));
                factory.AddProvider("trace", new TraceMetricsProvider(1, true));
                factory.AddProvider("console", new ConsoleMetricsProvider(1, true));

                var counter = factory.CreateCounter("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    counter.IncrementBy(10);
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = singleValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();

                Assert.IsTrue(itemList.Length >= 6, $"Single value count within groupings not within expected tolerance (value { itemList.Length })");
                Assert.IsTrue(itemList.Max(item => item.Value.Value) == 5000, "Maximum value count not expected");
            }

            Assert.IsTrue(summaryValues.Count == 0, "Summary value count not expected");
        }

        /// <summary>
        /// Counts up to 500
        /// </summary>
        [TestMethod]
        public void SimpleCountSetMultiProvider()
        {
            var singleValues = new ConcurrentBag<SingleMetricHolder>();
            var summaryValues = new ConcurrentBag<SummaryMetricHolder>();
            var testValue = 123.5678;

            using (var factory = new MetricFactory())
            {
                factory.AddCallback("callback1", 1, (object userState, string name, string[] labels, double? value, CancellationToken cancellationToken) =>
                {
                    singleValues.Add(new SingleMetricHolder { SampleTime = DateTime.UtcNow, Labels = labels, Name = name, UserState = userState, Value = value });
                    return Task.CompletedTask;
                },
                (object userState, string name, string[] labels, int? count, double? minimum, double? maximum, double? mean, double? standardDeviation, double? p50, double? p90, double? p95, double? p98, double? p99, CancellationToken cancellationToken) =>
                {
                    summaryValues.Add(new SummaryMetricHolder { SampleTime = DateTime.UtcNow, Labels = labels, Name = name, UserState = userState, Count = count, Minimum = minimum, Maximum = maximum, Mean = mean, StandardDeviation = standardDeviation, p50 = p50, p90 = p90, p95 = p95, p98 = p98, p99 = p99 });
                    return Task.CompletedTask;
                }, "test");

                factory.AddProvider("debug", new DebugMetricsProvider(1, true));
                factory.AddProvider("trace", new TraceMetricsProvider(1, true));
                factory.AddProvider("console", new ConsoleMetricsProvider(1, true));

                var counter = factory.CreateCounter("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    counter.SetTo(testValue);
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = singleValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();
                var maxValue = itemList.Max(item => item.Value);

                Assert.IsTrue(itemList.Length >= 6, $"Single value count within groupings not within expected tolerance (value { itemList.Length })");
                Assert.IsTrue(maxValue.HasValue, "Max value should not be null");
                Assert.IsTrue(maxValue == testValue, "Maximum value not expected");
            }

            Assert.IsTrue(summaryValues.Count == 0, "Summary value count not expected");
        }


        /// <summary>
        /// Counts up to 500
        /// </summary>
        [TestMethod]
        public void ValueConfirmation()
        {
            var callbackProvider = new CallbackMetricsProvider(1, (object userState, string name, string[] labels, double? value, CancellationToken cancellationToken) =>
            {
                return Task.CompletedTask;
            },
            (object userState, string name, string[] labels, int? count, double? minimum, double? maximum, double? mean, double? standardDeviation, double? p50, double? p90, double? p95, double? p98, double? p99, CancellationToken cancellationToken) =>
            {
                return Task.CompletedTask;
            }, "state");
            var callbackCounter = callbackProvider.CreateCounter("metricName1", "metricHelp", true, new string[] { "label1", "label2" });
            var callbackPulse = callbackProvider.CreatePulse("metricName2", "metricHelp", true, new string[] { "label1", "label2" });
            var callbackSummary = callbackProvider.CreateSummary("metricName3", "metricHelp", 10, true, new string[] { "label1", "label2" });
            var callbackGauge = callbackProvider.CreateGauge("metricName4", "metricHelp", true, new string[] { "label1", "label2" });

            var traceProvider = new TraceMetricsProvider(1, true);
            var traceCounter = traceProvider.CreateCounter("metricName1", "metricHelp", true, new string[] { "label1", "label2" });
            var tracePulse = traceProvider.CreatePulse("metricName2", "metricHelp", true, new string[] { "label1", "label2" });
            var traceSummary = traceProvider.CreateSummary("metricName3", "metricHelp", 10, true, new string[] { "label1", "label2" });
            var traceGauge = traceProvider.CreateGauge("metricName4", "metricHelp", true, new string[] { "label1", "label2" });

            var consoleProvider = new ConsoleMetricsProvider(1, true);
            var consoleCounter = consoleProvider.CreateCounter("metricName1", "metricHelp", true, new string[] { "label1", "label2" });
            var consolePulse = consoleProvider.CreatePulse("metricName2", "metricHelp", true, new string[] { "label1", "label2" });
            var consoleSummary = consoleProvider.CreateSummary("metricName3", "metricHelp", 10, true, new string[] { "label1", "label2" });
            var consoleGauge = consoleProvider.CreateGauge("metricName4", "metricHelp", true, new string[] { "label1", "label2" });

            var debugProvider = new DebugMetricsProvider(1, true);
            var debugCounter = debugProvider.CreateCounter("metricName1", "metricHelp", true, new string[] { "label1", "label2" });
            var debugPulse = debugProvider.CreatePulse("metricName2", "metricHelp", true, new string[] { "label1", "label2" });
            var debugSummary = debugProvider.CreateSummary("metricName3", "metricHelp", 10, true, new string[] { "label1", "label2" });
            var debugGauge = debugProvider.CreateGauge("metricName4", "metricHelp", true, new string[] { "label1", "label2" });

            var factory = new MetricFactory();
            var factoryCounter = factory.CreateCounter("metricName1", "metricHelp", true, new string[] { "label1", "label2" });
            var factoryPulse = factory.CreatePulse("metricName2", "metricHelp", true, new string[] { "label1", "label2" });
            var factorySummary = factory.CreateSummary("metricName3", "metricHelp", 10, true, new string[] { "label1", "label2" });
            var factoryGauge = factory.CreateGauge("metricName4", "metricHelp", true, new string[] { "label1", "label2" });


            Assert.IsTrue(string.Equals(callbackCounter.Name, "metricName1", StringComparison.Ordinal), "Callback counter name not expected");
            Assert.IsTrue(string.Equals(callbackCounter.Help, "metricHelp", StringComparison.Ordinal), "Callback counter help not expected");
            Assert.IsTrue(callbackCounter.Labels.Length == 2, "Callback counter labels not expected length");
            Assert.IsTrue(string.Equals(callbackCounter.Labels[0], "label1", StringComparison.Ordinal), "Callback counter label value 0 not expected");
            Assert.IsTrue(string.Equals(callbackCounter.Labels[1], "label2", StringComparison.Ordinal), "Callback counter label value 1 not expected");

            Assert.IsTrue(string.Equals(callbackPulse.Name, "metricName2", StringComparison.Ordinal), "Callback pulse name not expected");
            Assert.IsTrue(string.Equals(callbackPulse.Help, "metricHelp", StringComparison.Ordinal), "Callback pulse help not expected");
            Assert.IsTrue(callbackPulse.Labels.Length == 2, "Callback pulse labels not expected length");
            Assert.IsTrue(string.Equals(callbackPulse.Labels[0], "label1", StringComparison.Ordinal), "Callback pulse label value 0 not expected");
            Assert.IsTrue(string.Equals(callbackPulse.Labels[1], "label2", StringComparison.Ordinal), "Callback pulse label value 1 not expected");

            Assert.IsTrue(string.Equals(callbackSummary.Name, "metricName3", StringComparison.Ordinal), "Callback summary name not expected");
            Assert.IsTrue(string.Equals(callbackSummary.Help, "metricHelp", StringComparison.Ordinal), "Callback summary help not expected");
            Assert.IsTrue(callbackSummary.Labels.Length == 2, "Callback summary labels not expected length");
            Assert.IsTrue(string.Equals(callbackSummary.Labels[0], "label1", StringComparison.Ordinal), "Callback summary label value 0 not expected");
            Assert.IsTrue(string.Equals(callbackSummary.Labels[1], "label2", StringComparison.Ordinal), "Callback summary label value 1 not expected");

            Assert.IsTrue(string.Equals(callbackGauge.Name, "metricName4", StringComparison.Ordinal), "Callback gauge name not expected");
            Assert.IsTrue(string.Equals(callbackGauge.Help, "metricHelp", StringComparison.Ordinal), "Callback gauge help not expected");
            Assert.IsTrue(callbackGauge.Labels.Length == 2, "Callback gauge labels not expected length");
            Assert.IsTrue(string.Equals(callbackGauge.Labels[0], "label1", StringComparison.Ordinal), "Callback gauge label value 0 not expected");
            Assert.IsTrue(string.Equals(callbackGauge.Labels[1], "label2", StringComparison.Ordinal), "Callback gauge label value 1 not expected");



            Assert.IsTrue(string.Equals(traceCounter.Name, "metricName1", StringComparison.Ordinal), "Trace counter name not expected");
            Assert.IsTrue(string.Equals(traceCounter.Help, "metricHelp", StringComparison.Ordinal), "Trace counter help not expected");
            Assert.IsTrue(traceCounter.Labels.Length == 2, "Trace counter labels not expected length");
            Assert.IsTrue(string.Equals(traceCounter.Labels[0], "label1", StringComparison.Ordinal), "Trace counter label value 0 not expected");
            Assert.IsTrue(string.Equals(traceCounter.Labels[1], "label2", StringComparison.Ordinal), "Trace counter label value 1 not expected");

            Assert.IsTrue(string.Equals(tracePulse.Name, "metricName2", StringComparison.Ordinal), "Trace pulse name not expected");
            Assert.IsTrue(string.Equals(tracePulse.Help, "metricHelp", StringComparison.Ordinal), "Trace pulse help not expected");
            Assert.IsTrue(tracePulse.Labels.Length == 2, "Trace pulse labels not expected length");
            Assert.IsTrue(string.Equals(tracePulse.Labels[0], "label1", StringComparison.Ordinal), "Trace pulse label value 0 not expected");
            Assert.IsTrue(string.Equals(tracePulse.Labels[1], "label2", StringComparison.Ordinal), "Trace pulse label value 1 not expected");

            Assert.IsTrue(string.Equals(traceSummary.Name, "metricName3", StringComparison.Ordinal), "Trace summary name not expected");
            Assert.IsTrue(string.Equals(traceSummary.Help, "metricHelp", StringComparison.Ordinal), "Trace summary help not expected");
            Assert.IsTrue(traceSummary.Labels.Length == 2, "Trace summary labels not expected length");
            Assert.IsTrue(string.Equals(traceSummary.Labels[0], "label1", StringComparison.Ordinal), "Trace summary label value 0 not expected");
            Assert.IsTrue(string.Equals(traceSummary.Labels[1], "label2", StringComparison.Ordinal), "Trace summary label value 1 not expected");

            Assert.IsTrue(string.Equals(traceGauge.Name, "metricName4", StringComparison.Ordinal), "Trace gauge name not expected");
            Assert.IsTrue(string.Equals(traceGauge.Help, "metricHelp", StringComparison.Ordinal), "Trace gauge help not expected");
            Assert.IsTrue(traceGauge.Labels.Length == 2, "Trace gauge labels not expected length");
            Assert.IsTrue(string.Equals(traceGauge.Labels[0], "label1", StringComparison.Ordinal), "Trace gauge label value 0 not expected");
            Assert.IsTrue(string.Equals(traceGauge.Labels[1], "label2", StringComparison.Ordinal), "Trace gauge label value 1 not expected");




            Assert.IsTrue(string.Equals(consoleCounter.Name, "metricName1", StringComparison.Ordinal), "Console counter name not expected");
            Assert.IsTrue(string.Equals(consoleCounter.Help, "metricHelp", StringComparison.Ordinal), "Console counter help not expected");
            Assert.IsTrue(consoleCounter.Labels.Length == 2, "Console counter labels not expected length");
            Assert.IsTrue(string.Equals(consoleCounter.Labels[0], "label1", StringComparison.Ordinal), "Console counter label value 0 not expected");
            Assert.IsTrue(string.Equals(consoleCounter.Labels[1], "label2", StringComparison.Ordinal), "Console counter label value 1 not expected");

            Assert.IsTrue(string.Equals(consolePulse.Name, "metricName2", StringComparison.Ordinal), "Console pulse name not expected");
            Assert.IsTrue(string.Equals(consolePulse.Help, "metricHelp", StringComparison.Ordinal), "Console pulse help not expected");
            Assert.IsTrue(consolePulse.Labels.Length == 2, "Console pulse labels not expected length");
            Assert.IsTrue(string.Equals(consolePulse.Labels[0], "label1", StringComparison.Ordinal), "Console pulse label value 0 not expected");
            Assert.IsTrue(string.Equals(consolePulse.Labels[1], "label2", StringComparison.Ordinal), "Console pulse label value 1 not expected");

            Assert.IsTrue(string.Equals(consoleSummary.Name, "metricName3", StringComparison.Ordinal), "Console summary name not expected");
            Assert.IsTrue(string.Equals(consoleSummary.Help, "metricHelp", StringComparison.Ordinal), "Console summary help not expected");
            Assert.IsTrue(consoleSummary.Labels.Length == 2, "Console summary labels not expected length");
            Assert.IsTrue(string.Equals(consoleSummary.Labels[0], "label1", StringComparison.Ordinal), "Console summary label value 0 not expected");
            Assert.IsTrue(string.Equals(consoleSummary.Labels[1], "label2", StringComparison.Ordinal), "Console summary label value 1 not expected");

            Assert.IsTrue(string.Equals(consoleGauge.Name, "metricName4", StringComparison.Ordinal), "Console gauge name not expected");
            Assert.IsTrue(string.Equals(consoleGauge.Help, "metricHelp", StringComparison.Ordinal), "Console gauge help not expected");
            Assert.IsTrue(consoleGauge.Labels.Length == 2, "Console gauge labels not expected length");
            Assert.IsTrue(string.Equals(consoleGauge.Labels[0], "label1", StringComparison.Ordinal), "Console gauge label value 0 not expected");
            Assert.IsTrue(string.Equals(consoleGauge.Labels[1], "label2", StringComparison.Ordinal), "Console gauge label value 1 not expected");



            Assert.IsTrue(string.Equals(debugCounter.Name, "metricName1", StringComparison.Ordinal), "Debug counter name not expected");
            Assert.IsTrue(string.Equals(debugCounter.Help, "metricHelp", StringComparison.Ordinal), "Debug counter help not expected");
            Assert.IsTrue(debugCounter.Labels.Length == 2, "Debug counter labels not expected length");
            Assert.IsTrue(string.Equals(debugCounter.Labels[0], "label1", StringComparison.Ordinal), "Debug counter label value 0 not expected");
            Assert.IsTrue(string.Equals(debugCounter.Labels[1], "label2", StringComparison.Ordinal), "Debug counter label value 1 not expected");

            Assert.IsTrue(string.Equals(debugPulse.Name, "metricName2", StringComparison.Ordinal), "Debug pulse name not expected");
            Assert.IsTrue(string.Equals(debugPulse.Help, "metricHelp", StringComparison.Ordinal), "Debug pulse help not expected");
            Assert.IsTrue(debugPulse.Labels.Length == 2, "Debug pulse labels not expected length");
            Assert.IsTrue(string.Equals(debugPulse.Labels[0], "label1", StringComparison.Ordinal), "Debug pulse label value 0 not expected");
            Assert.IsTrue(string.Equals(debugPulse.Labels[1], "label2", StringComparison.Ordinal), "Debug pulse label value 1 not expected");

            Assert.IsTrue(string.Equals(debugSummary.Name, "metricName3", StringComparison.Ordinal), "Debug summary name not expected");
            Assert.IsTrue(string.Equals(debugSummary.Help, "metricHelp", StringComparison.Ordinal), "Debug summary help not expected");
            Assert.IsTrue(debugSummary.Labels.Length == 2, "Debug summary labels not expected length");
            Assert.IsTrue(string.Equals(debugSummary.Labels[0], "label1", StringComparison.Ordinal), "Debug summary label value 0 not expected");
            Assert.IsTrue(string.Equals(debugSummary.Labels[1], "label2", StringComparison.Ordinal), "Debug summary label value 1 not expected");

            Assert.IsTrue(string.Equals(debugGauge.Name, "metricName4", StringComparison.Ordinal), "Debug gauge name not expected");
            Assert.IsTrue(string.Equals(debugGauge.Help, "metricHelp", StringComparison.Ordinal), "Debug gauge help not expected");
            Assert.IsTrue(debugGauge.Labels.Length == 2, "Debug gauge labels not expected length");
            Assert.IsTrue(string.Equals(debugGauge.Labels[0], "label1", StringComparison.Ordinal), "Debug gauge label value 0 not expected");
            Assert.IsTrue(string.Equals(debugGauge.Labels[1], "label2", StringComparison.Ordinal), "Debug gauge label value 1 not expected");


            Assert.IsTrue(string.Equals(factoryCounter.Name, "metricName1", StringComparison.Ordinal), "Factory counter name not expected");
            Assert.IsTrue(string.Equals(factoryCounter.Help, "metricHelp", StringComparison.Ordinal), "Factory counter help not expected");
            Assert.IsTrue(factoryCounter.Labels.Length == 2, "Factory counter labels not expected length");
            Assert.IsTrue(string.Equals(factoryCounter.Labels[0], "label1", StringComparison.Ordinal), "Factory counter label value 0 not expected");
            Assert.IsTrue(string.Equals(factoryCounter.Labels[1], "label2", StringComparison.Ordinal), "Factory counter label value 1 not expected");

            Assert.IsTrue(string.Equals(factoryPulse.Name, "metricName2", StringComparison.Ordinal), "Factory pulse name not expected");
            Assert.IsTrue(string.Equals(factoryPulse.Help, "metricHelp", StringComparison.Ordinal), "Factory pulse help not expected");
            Assert.IsTrue(factoryPulse.Labels.Length == 2, "Factory pulse labels not expected length");
            Assert.IsTrue(string.Equals(factoryPulse.Labels[0], "label1", StringComparison.Ordinal), "Factory pulse label value 0 not expected");
            Assert.IsTrue(string.Equals(factoryPulse.Labels[1], "label2", StringComparison.Ordinal), "Factory pulse label value 1 not expected");

            Assert.IsTrue(string.Equals(factorySummary.Name, "metricName3", StringComparison.Ordinal), "Factory summary name not expected");
            Assert.IsTrue(string.Equals(factorySummary.Help, "metricHelp", StringComparison.Ordinal), "Factory summary help not expected");
            Assert.IsTrue(factorySummary.Labels.Length == 2, "Factory summary labels not expected length");
            Assert.IsTrue(string.Equals(factorySummary.Labels[0], "label1", StringComparison.Ordinal), "Factory summary label value 0 not expected");
            Assert.IsTrue(string.Equals(factorySummary.Labels[1], "label2", StringComparison.Ordinal), "Factory summary label value 1 not expected");

            Assert.IsTrue(string.Equals(factoryGauge.Name, "metricName4", StringComparison.Ordinal), "Factory gauge name not expected");
            Assert.IsTrue(string.Equals(factoryGauge.Help, "metricHelp", StringComparison.Ordinal), "Factory gauge help not expected");
            Assert.IsTrue(factoryGauge.Labels.Length == 2, "Factory gauge labels not expected length");
            Assert.IsTrue(string.Equals(factoryGauge.Labels[0], "label1", StringComparison.Ordinal), "Factory gauge label value 0 not expected");
            Assert.IsTrue(string.Equals(factoryGauge.Labels[1], "label2", StringComparison.Ordinal), "Factory gauge label value 1 not expected");

        }
        #endregion
    }
}
