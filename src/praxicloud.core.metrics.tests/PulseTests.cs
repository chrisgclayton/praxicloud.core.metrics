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
    public class PulseTests
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

                var counter = factory.CreatePulse("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

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

                var pulse = factory.CreatePulse("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    pulse.Observe();
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = singleValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();
                var mean = itemList.Average(item => item.Value ?? 0);

                Assert.IsTrue(itemList.Length >= 6, $"Single value count within groupings not within expected tolerance (value { itemList.Length })");
                Assert.IsTrue(mean >= 50, $"Mean value count not in tolerance (value { mean })");
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

                var pulse = factory.CreatePulse("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    pulse.Observe();
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = singleValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();
                var mean = itemList.Average(item => item.Value ?? 0);

                Assert.IsTrue(itemList.Length >= 6, $"Single value count within groupings not within expected tolerance (value { itemList.Length })");
                Assert.IsTrue(mean >= 50, $"Mean value count not in tolerance (value { mean })");
            }

            Assert.IsTrue(summaryValues.Count == 0, "Summary value count not expected");
        }
        #endregion
    }
}
