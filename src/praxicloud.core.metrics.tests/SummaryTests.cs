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
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// A set of tests to validate the counters
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SummaryTests
    {
        #region Variables
        /// <summary>
        /// A known value of doubles
        /// </summary>
        public readonly double[] _doubleValues = new double[] { 67.0, 77.7, 97.4, 23.3, 41.3, 81.8, 64.7, 99.6, 24.8, 49.6, 58.6, 35.2, 53.2, 26.6, 16.8, 15.1, 35.6, 70.0, 49.7, 9.1, 41.1, 92.7, 36.2, 12.0, 50.8, 38.3, 17.2, 52.6, 13.4, 63.1, 46.2, 45.0, 78.4, 56.2, 43.5, 3.9, 45.5, 32.6, 51.7, 23.0, 53.6, 72.5, 96.1, 11.4, 68.3, 32.3, 61.1, 95.3, 31.0, 37.6, 99.7, 8.6, 40.8, 40.3, 18.7, 74.3, 41.5, 85.5, 72.5, 3.0, 64.6, 12.2, 86.2, 61.7, 3.4, 13.6, 56.8, 96.9, 83.1, 13.0, 69.6, 3.0, 18.3, 26.7, 55.5, 68.6, 96.6, 24.9, 43.7, 18.5, 77.2, 21.9, 21.5, 75.7, 63.4, 37.6, 4.2, 38.1, 37.5, 69.3, 62.4, 60.1, 60.1, 48.7, 58.4, 41.4, 86.3, 34.4, 8.0, 54.7, 75.5, 64.1, 12.3, 18.1, 68.1, 96.6, 90.2, 14.1, 84.8, 50.2, 77.7, 16.6, 41.8, 53.9, 25.9, 42.7, 90.6, 10.5, 98.3, 65.8, 9.4, 18.8, 59.4, 13.8, 50.6, 9.5, 42.9, 69.6, 68.3, 14.1, 82.3, 62.2, 16.9, 89.0, 43.1, 13.0, 9.6, 3.5, 7.6, 66.8, 47.4, 90.1, 53.3, 87.3, 91.5, 45.8, 18.3, 6.2, 22.8, 15.7, 50.8, 75.8, 36.1, 42.2, 45.3, 56.7, 4.7, 98.5, 67.5, 58.6, 53.8, 20.6, 45.8, 70.7, 67.9, 15.6, 99.7, 52.8, 10.9, 12.8, 33.0, 87.2, 2.8, 31.5, 18.4, 19.3, 65.5, 72.1, 22.3, 4.8, 91.2, 36.7, 46.8, 52.6, 63.3, 6.4, 26.0, 74.7, 43.7, 86.4, 8.3, 11.2, 36.0, 49.1, 13.0, 26.8, 44.3, 82.5, 19.4, 76.0, 46.1, 65.6, 95.3, 10.0, 82.6, 63.6, 73.0, 4.6, 23.7, 25.9, 91.2, 32.6, 76.2, 62.7, 67.4, 17.1, 73.8, 93.2, 7.5, 61.5, 89.5, 25.0, 9.0, 24.5, 4.5, 21.9, 51.2, 53.8, 18.5, 91.6, 75.1, 83.0, 52.7, 46.3, 58.7, 25.6, 41.3, 36.7, 70.0, 99.7, 33.4, 21.4, 51.0, 17.8, 95.2, 75.7, 35.0, 73.3, 81.6, 95.9, 52.9, 51.1, 75.0, 58.0, 86.4, 21.1, 56.6, 70.8, 5.6, 60.8, 12.4, 19.2, 86.2, 32.1, 50.8, 8.2, 80.0, 29.9, 4.0, 41.8, 75.8, 37.1, 23.2, 7.8, 28.1, 68.2, 74.0, 91.2, 29.3, 28.7, 86.9, 77.9, 72.1, 22.6, 38.7, 24.0, 8.0, 94.7, 59.9, 37.6, 69.0, 70.5, 31.2, 9.2, 87.2, 14.2, 35.2, 18.9, 67.0, 87.0, 95.8, 5.1, 69.3, 39.8, 20.1, 15.8, 27.8, 67.2, 29.9, 18.3, 47.2, 65.4, 41.5, 76.8, 73.9, 34.6, 47.1, 63.6, 93.4, 26.8, 0.2, 85.3, 70.0, 66.4, 72.8, 5.2, 5.9, 14.1, 20.6, 13.8, 32.9, 55.1, 24.2, 42.3, 33.0, 81.7, 8.5, 32.3, 2.4, 22.9, 96.2, 40.7, 64.8, 41.6, 90.4, 3.7, 29.0, 54.3, 35.2, 52.7, 67.2, 71.7, 25.5, 40.2, 86.8, 10.5, 35.1, 2.9, 67.1, 14.9, 9.9, 13.8, 46.7, 16.1, 85.3, 92.1, 41.2, 99.2, 43.9, 92.2, 26.0, 14.8, 61.1, 70.5, 30.6, 59.5, 20.4, 28.4, 76.0, 69.1, 76.2, 51.6, 78.8, 67.9, 46.7, 61.3, 29.5, 84.1, 55.5, 22.4, 46.6, 5.6, 65.3, 87.5, 13.0, 82.4, 94.1, 48.8, 56.3, 98.3, 62.5, 29.8, 10.4, 43.0, 26.7, 52.4, 10.5, 55.1, 9.5, 27.4, 90.1, 6.7, 26.9, 97.9, 38.7, 58.3, 35.0, 78.8, 69.4, 24.0, 62.6, 57.1, 43.7, 21.4, 45.6, 20.4, 49.5, 73.6, 57.5, 48.2, 65.4, 71.7, 72.1, 77.7, 6.6, 46.4, 41.2, 35.8, 41.3, 94.3, 50.8, 74.5, 74.5, 28.1, 32.4, 40.0, 78.7, 67.3, 48.8, 54.7, 47.4, 15.3, 79.4, 32.3, 35.7, 5.4, 86.1, 89.0, 97.5, 6.3, 2.9, 36.9, 97.6, 61.3, 62.0, 18.4, 34.6, 49.2, 91.4, 92.3, 17.1, 99.2, 37.5, 75.1, 73.3, 88.1, 82.6, 15.6, 89.1, 5.6, 41.7, 82.2, 24.8, 2.8, 0.8, 50.1, 92.3, 39.8, 42.0, 1.2, 60.3, 52.2, 38.3, 35.0, 91.4, 13.9, 76.9, 13.3, 66.2, 14.0 };
        #endregion
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

                var counter = factory.CreateSummary("Metric1", "Test metric for #1", 1, true, new string[] { "label1", "label2" });

                Task.Delay(5000).GetAwaiter().GetResult();
            }

            Assert.IsTrue(summaryValues.IsEmpty, "Single value count not expected");
            Assert.IsTrue(summaryValues.IsEmpty, "Summary value count not expected");
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

                var summary = factory.CreateSummary("Metric1", "Test metric for #1", 5, true, new string[] { "label1", "label2" });

                for (var index = 0; index < 1500; index++)
                {
                    summary.Observe(_doubleValues[index % _doubleValues.Length]);
                    if (index < 1499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = summaryValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            var start = 0;

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();

                start += itemList.Length;
            }

            Assert.IsTrue(singleValues.IsEmpty, "single value count not expected");
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

                var summary = factory.CreateSummary("Metric1", "Test metric for #1", 5, true, new string[] { "label1", "label2" });

                for (var index = 0; index < 1500; index++)
                {
                    summary.Observe(_doubleValues[index % _doubleValues.Length]);
                    if (index < 1499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = summaryValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            var start = 0;

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();

                start += itemList.Length;
            }

            Assert.IsTrue(singleValues.IsEmpty, "single value count not expected");
        }

        /// <summary>
        /// Counts up to 500
        /// </summary>
        [TestMethod]
        public void FactoryCreateCountIteration()
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

                factory.AddDebug("debug", 1, true);
                factory.AddTrace("trace", 1, true);
                factory.AddConsole("console", 1, true);

                var summary = factory.CreateSummary("Metric1", "Test metric for #1", 5, true, new string[] { "label1", "label2" });

                for (var index = 0; index < 1500; index++)
                {
                    summary.Observe(_doubleValues[index % _doubleValues.Length]);
                    if (index < 1499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = summaryValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            var start = 0;

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();

                start += itemList.Length;
            }

            Assert.IsTrue(singleValues.IsEmpty, "single value count not expected");
        }


        /// <summary>
        /// Times 100 values
        /// </summary>
        [TestMethod]
        public void SimpleTimer()
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

                // Added to force aggregated to be added while allowing for callback tracking
                factory.AddDebug("debug", 1, false);

                var summary = factory.CreateSummary("Metric1", "Test metric for #1", 5, true, new string[] { "label1", "label2" });

                for (var index = 0; index < 1000; index++)
                {
                    using (summary.Time())
                    {
                        Task.Delay(10).GetAwaiter().GetResult();
                    }
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = summaryValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            var start = 0;

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();

                var average = itemList.Average(item => item.p50);

                Assert.IsTrue(itemList.Length > 5, "More than 5 items were expected");
//                Assert.IsTrue(average >= 10 && average <= 20, "Average was not expected when more than 5 values in frame");

                start += itemList.Length;
            }

            Assert.IsTrue(singleValues.IsEmpty, "single value count not expected");
        }

        /// <summary>
        /// Bucket writes aggregates
        /// </summary>
        [TestMethod]
        public void BucketWriter()
        {
            var bucket = new MetricBucket(1);

            bucket.GetAggregates(out var startTime, out var count, out var maximum, out var minimum, out var mean, out var standardDeviation, out var p50, out var p90, out var p95, out var p98, out var p99);

            Assert.IsNull(startTime, "Start Time not expected");
            Assert.IsNull(count, "Count not expected");
            Assert.IsNull(maximum, "Maximum not expected");
            Assert.IsNull(minimum, "Minimum not expected");
            Assert.IsNull(mean, "Mean not expected");
            Assert.IsNull(standardDeviation, "Standard deviation not expected");
            Assert.IsNull(p50, "P50 not expected");
            Assert.IsNull(p90, "P90 not expected");
            Assert.IsNull(p95, "P95 not expected");
            Assert.IsNull(p98, "P98 not expected");
            Assert.IsNull(p99, "P99 not expected");
        }

        #endregion
    }
}
