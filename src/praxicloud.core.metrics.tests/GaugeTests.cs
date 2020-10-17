// Copyright (c) Chris Clayton. All rights reserved.
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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;
    #endregion

    /// <summary>
    /// A set of tests to validate the metrics gauge
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class GaugeTests
    {
        #region Simple Gauge
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

                var counter = factory.CreateGauge("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                Task.Delay(5000).GetAwaiter().GetResult();
            }

            Assert.IsTrue(summaryValues.Count == 0, "Single value count not expected");
            Assert.IsTrue(summaryValues.Count == 0, "Summary value count not expected");
        }

        /// <summary>
        /// Counts up to 500
        /// </summary>
        [TestMethod]
        public void SimpleGaugeIteration()
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

                var gauge = factory.CreateGauge("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {                   
                    gauge.Increment();
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
        /// Counts up to 5000 by 10
        /// </summary>
        [TestMethod]
        public void SimpleGaugeIterationBy()
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

                var gauge = factory.CreateGauge("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    gauge.IncrementBy(10);
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
        /// Counts up to 500 and back to 0
        /// </summary>
        [TestMethod]
        public void SimpleGaugeIncreaseDecrease()
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

                var gauge = factory.CreateGauge("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    gauge.Increment();
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();

                for (var index = 0; index < 500; index++)
                {
                    gauge.Decrement();
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = singleValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();

                Assert.IsTrue(itemList.Length >= 12, "Single value count within groupings not within expected tolerance");
                Assert.IsTrue((itemList.Max(item => item.Value ?? 0)) >= 450, "Maximum value count not expected");
                Assert.IsTrue(itemList[itemList.Length - 1].Value == 0, "Return to zero missed");
            }

            Assert.IsTrue(summaryValues.Count == 0, "Summary value count not expected");
        }

        /// <summary>
        /// Counts up to 5000 and back to 0
        /// </summary>
        [TestMethod]
        public void SimpleGaugeIncreaseDecreaseBy()
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

                var gauge = factory.CreateGauge("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    gauge.IncrementBy(10);
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();

                for (var index = 0; index < 500; index++)
                {
                    gauge.DecrementBy(10);
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = singleValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();

                Assert.IsTrue(itemList.Length >= 12, $"Single value count within groupings not within expected tolerance (value { itemList.Length })");
                Assert.IsTrue(itemList.Max(item => item.Value.Value) == 5000, "Maximum value count not expected");
                Assert.IsTrue(itemList[itemList.Length - 1].Value == 0, "Return to zero missed");
            }

            Assert.IsTrue(summaryValues.Count == 0, "Summary value count not expected");
        }

        /// <summary>
        /// Counts up to 500
        /// </summary>
        [TestMethod]
        public void SimpleGaugeIterationMultiProvider()
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

                var gauge = factory.CreateGauge("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    gauge.Increment();
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
        /// Counts up to 5000 by 10
        /// </summary>
        [TestMethod]
        public void SimpleGaugeIterationByMultiProvider()
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

                var gauge = factory.CreateGauge("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    gauge.IncrementBy(10);
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
        /// Counts up to 500 and back to 0
        /// </summary>
        [TestMethod]
        public void SimpleGaugeIncreaseDecreaseMultiProvider()
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

                var gauge = factory.CreateGauge("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    gauge.Increment();
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();

                for (var index = 0; index < 500; index++)
                {
                    gauge.Decrement();
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = singleValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();

                Assert.IsTrue(itemList.Length >= 12, $"Single value count within groupings not within expected tolerance (value { itemList.Length })");
                Assert.IsTrue((itemList.Max(item => item.Value ?? 0)) >= 450, "Maximum value count not expected");
                Assert.IsTrue(itemList[itemList.Length - 1].Value == 0, "Return to zero missed");
            }

            Assert.IsTrue(summaryValues.Count == 0, "Summary value count not expected");
        }

        /// <summary>
        /// Counts up to 5000 and back to 0
        /// </summary>
        [TestMethod]
        public void SimpleGaugeIncreaseDecreaseByMultiProvider()
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

                var gauge = factory.CreateGauge("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    gauge.IncrementBy(10);
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();

                for (var index = 0; index < 500; index++)
                {
                    gauge.DecrementBy(10);
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = singleValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();

                Assert.IsTrue(itemList.Length >= 2, $"Single value count within groupings not within expected tolerance (value { itemList.Length })");
                Assert.IsTrue(itemList.Max(item => item.Value.Value) == 5000, "Maximum value count not expected");
                Assert.IsTrue(itemList[itemList.Length - 1].Value == 0, "Return to zero missed");
            }

            Assert.IsTrue(summaryValues.Count == 0, "Summary value count not expected");
        }



        /// <summary>
        /// Counts up to 5000 and back to 0
        /// </summary>
        [TestMethod]
        public void SimpleGaugeSetValueByMultiProvider()
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

                var gauge = factory.CreateGauge("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                for (var index = 0; index < 500; index++)
                {
                    gauge.SetTo(456.789);
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();
            }

            var groupings = singleValues.GroupBy(item => item.Name).ToArray();

            Assert.IsTrue(groupings.Length == 1, "Metric count not expected");

            foreach (var item in groupings)
            {
                var itemList = item.OrderBy(item => item.SampleTime).ToArray();

                Assert.IsTrue(itemList.Length >= 5, "Single value count within groupings not within expected tolerance");
                Assert.IsTrue(itemList.Max(item => item.Value.Value) == 456.789, "Maximum value count not expected");
            }

            Assert.IsTrue(summaryValues.Count == 0, "Summary value count not expected");
        }


        /// <summary>
        /// Tracks 500 operations values
        /// </summary>
        [TestMethod]
        public void SimpleGaugeTracker()
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
                var gauge = factory.CreateGauge("Metric1", "Test metric for #1", true, new string[] { "label1", "label2" });

                var disposalList = new List<IDisposable>();

                for (var index = 0; index < 500; index++)
                {
                    disposalList.Add(gauge.TrackExecution());
                    if (index < 499) Task.Delay(10).GetAwaiter().GetResult();
                }

                Task.Delay(1000).GetAwaiter().GetResult();

                var maximumConcurrency = singleValues.Max(item => item.Value.Value);

                foreach(var item in disposalList)
                {
                    item.Dispose();
                }

                Task.Delay(1000).GetAwaiter().GetResult();

                var orderedValues = singleValues.OrderBy(item => item.SampleTime).ToArray();

//                Assert.IsTrue(orderedValues[orderedValues.Length - 1].Value > 50, $"The gauge final result was not expected (value {orderedValues[orderedValues.Length - 1].Value})");
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

        #endregion
    }
}
