// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.simpleprovider
{
    #region Using Clauses
    using praxicloud.core.security;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// An provider for metrics that invokes callback methods to report the values
    /// </summary>
    public abstract class SimpleMetricsProvider : IMetricProvider, ISimpleMetricWriter, IDisposable
    {
        #region Variables
        /// <summary>
        /// The cancellation token to monitor for disposal
        /// </summary>
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();

        /// <summary>
        /// A list of metrics to manage
        /// </summary>
        private readonly ConcurrentBag<ISimpleMetric> _metrics = new ConcurrentBag<ISimpleMetric>();
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="reportingInterval">The number of seconds before each call to counters to report values</param>
        /// <param name="userState">A user state object that consumers can provide that is meaningful to their callbacks</param>
        public SimpleMetricsProvider(long reportingInterval, object userState)
        {
            Guard.NotLessThan(nameof(reportingInterval), reportingInterval, 1);

            UserState = userState;          
            ProcessingTask = ProcessMessagesAsync(reportingInterval);
        }
        #endregion
        #region Properties
        /// <summary>
        /// The user state object provided
        /// </summary>
        protected object UserState { get; }

        /// <summary>
        /// The task that runs while processing occurs
        /// </summary>
        protected Task ProcessingTask { get; }

        /// <summary>
        /// The cancellation in use for the provider
        /// </summary>
        protected CancellationToken CancellationToken => _cancellation.Token;

        /// <summary>
        /// True as long as the object is to continue recording
        /// </summary>
        protected bool ContinueProcessing { get; private set; } = true;
        #endregion
        #region Methods
        /// <summary>
        /// A loop that reports at regular intervals
        /// </summary>
        /// <param name="reportingInterval">The interval to report in measured in seconds</param>
        private async Task ProcessMessagesAsync(long reportingInterval)
        {
            var nextReport = DateTimeOffset.UtcNow.AddSeconds(reportingInterval); ;

            do
            {
                if (DateTimeOffset.UtcNow >= nextReport)
                {
                    nextReport = nextReport.AddSeconds(reportingInterval);

                    var reporting = new List<Task>();

                    foreach(var metric in _metrics)
                    {
                        reporting.Add(metric.WriteAsync(_cancellation.Token));
                    }

                    await Task.WhenAll(reporting).ConfigureAwait(false);
                }

                await Task.Delay(100).ConfigureAwait(false);
            } while (ContinueProcessing && !_cancellation.IsCancellationRequested);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ContinueProcessing = false;

            _cancellation.CancelAfter(3000);
            Task.WhenAny(Task.Delay(3000), ProcessingTask).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public ICounter CreateCounter(string name, string help, bool delayPublish, string[] labels)
        {
            MetricPrecreate(name, help, delayPublish, labels);
            var metric = new SimpleCounter(this, UserState, delayPublish, name, help, labels);

            _metrics.Add(metric);
            MetricPostcreate(name, help, delayPublish, labels, metric);

            return metric;
        }

        /// <inheritdoc />
        public IGauge CreateGauge(string name, string help, bool delayPublish, string[] labels)
        {
            MetricPrecreate(name, help, delayPublish, labels);
            var metric = new SimpleGauge(this, UserState, delayPublish, name, help, labels);

            _metrics.Add(metric);
            MetricPostcreate(name, help, delayPublish, labels, metric);

            return metric;
        }

        /// <inheritdoc />
        public IPulse CreatePulse(string name, string help, bool delayPublish, string[] labels)
        {
            MetricPrecreate(name, help, delayPublish, labels);
            var metric = new SimplePulse(this, UserState, delayPublish, name, help, labels);

            _metrics.Add(metric);
            MetricPostcreate(name, help, delayPublish, labels, metric);

            return metric;
        }

        /// <inheritdoc />
        public ISummary CreateSummary(string name, string help, long duration, bool delayPublish, string[] labels)
        {
            MetricPrecreate(name, help, delayPublish, labels);
            var metric = new SimpleSummary(this, UserState, delayPublish, duration, name, help, labels);

            _metrics.Add(metric);
            MetricPostcreate(name, help, delayPublish, labels, metric);

            return metric;
        }

        /// <summary>
        /// A method that can be overridden to perform processing before the metric is created
        /// </summary>
        /// <param name="name">The name of the metric</param>
        /// <param name="help">The help text associated with the metric</param>
        /// <param name="delayPublish">True if the value should delay publishing until it has a value written to it</param>
        /// <param name="labels">Labels associated with the metric that may demonstrate different views. It is up to the metric provider whether it is supported and the number of labels that are supported, no exceptions will be raised as a result but many providers will demonstrate a performance impact for too many labels as it may create descrete metrics.</param>
        protected virtual void MetricPrecreate(string name, string help, bool delayPublish, string[] labels)
        {

        }

        /// <summary>
        /// A method that can be overridden to perform processing after the metric is created
        /// </summary>
        /// <param name="name">The name of the metric</param>
        /// <param name="help">The help text associated with the metric</param>
        /// <param name="delayPublish">True if the value should delay publishing until it has a value written to it</param>
        /// <param name="labels">Labels associated with the metric that may demonstrate different views. It is up to the metric provider whether it is supported and the number of labels that are supported, no exceptions will be raised as a result but many providers will demonstrate a performance impact for too many labels as it may create descrete metrics.</param>
        /// <param name="metricInstance">The instance that was created</param>
        protected virtual void MetricPostcreate(string name, string help, bool delayPublish, string[] labels, ISimpleMetric metricInstance)
        {

        }

        /// <summary>
        /// A writer that is called each time the metric is to be written
        /// </summary>
        /// <param name="name">The name of the metric</param>
        /// <param name="userState">A user state object that was provided to the provider</param>
        /// <param name="labels">The labels associated with the metric</param>
        /// <param name="value">The value that the metric is currently at</param>
        /// <param name="cancellationToken">A token to monitor for abort requests</param>
        public abstract Task MetricWriterSingleValueAsync(object userState, string name, string[] labels, double? value, CancellationToken cancellationToken);

        /// <summary>
        /// A writer that is called each time the metric is to be written
        /// </summary>
        /// <param name="name">The name of the metric</param>
        /// <param name="userState">A user state object that was provided to the provider</param>
        /// <param name="labels">The labels associated with the metric</param>
        /// <param name="count">The number of values</param>
        /// <param name="maximum">The maximum value</param>
        /// <param name="mean">The average value using a mean aggregation</param>
        /// <param name="minimum">The minimum value</param>
        /// <param name="p50">The 50th percentile</param>
        /// <param name="p90">The 90th percentile</param>
        /// <param name="p95">The 95th percentile</param>
        /// <param name="p98">The 98th percentile</param>
        /// <param name="p99">The 99th percentile</param>
        /// <param name="standardDeviation">The standard deviation</param>
                /// <param name="cancellationToken">A token to monitor for abort requests</param>
        public abstract Task MetricWriterSummaryAsync(object userState, string name, string[] labels, int? count, double? minimum, double? maximum, double? mean, double? standardDeviation, double? p50, double? p90, double? p95, double? p98, double? p99, CancellationToken cancellationToken);
        #endregion
    }
}
