// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.callbackprovider
{
    #region Using Clauses
    using praxicloud.core.security;
    using praxicloud.core.metrics.simpleprovider;
    using System.Threading;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// An provider for metrics that invokes callback methods to report the values
    /// </summary>
    public sealed class CallbackMetricsProvider : SimpleMetricsProvider
    {
        #region Delegates
        /// <summary>
        /// A writer that is called each time the metric is to be written
        /// </summary>
        /// <param name="name">The name of the metric</param>
        /// <param name="userState">A user state object that was provided to the provider</param>
        /// <param name="labels">The labels associated with the metric</param>
        /// <param name="value">The value that the metric is currently at</param>
        /// <param name="cancellationToken">A token to monitor for abort requests</param>
        public delegate Task MetricWriterSingleValue(object userState, string name, string[] labels, double? value, CancellationToken cancellationToken);

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
        public delegate Task MetricWriterSummary(object userState, string name, string[] labels, int? count, double? minimum, double? maximum, double? mean, double? standardDeviation, double? p50, double? p90, double? p95, double? p98, double? p99, CancellationToken cancellationToken);
        #endregion
        #region Variables
        /// <summary>
        /// A callback method invoked to write single values
        /// </summary>
        private readonly MetricWriterSingleValue _singleValueWriter;

        /// <summary>
        /// A callback method invoked to write summary data
        /// </summary>
        private readonly MetricWriterSummary _summaryWriter;
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="reportingInterval">The number of seconds before each call to counters to report values</param>
        /// <param name="singleValueWriter">The callback method that is invoked for single value writes</param>
        /// <param name="summaryWriter">The callback method that is invoked for summary data writes</param>
        /// <param name="userState">A user state object that consumers can provide that is meaningful to their callbacks</param>
        public CallbackMetricsProvider(long reportingInterval, MetricWriterSingleValue singleValueWriter, MetricWriterSummary summaryWriter, object userState) : base(reportingInterval, userState)
        {
            Guard.NotLessThan(nameof(reportingInterval), reportingInterval, 1);
            Guard.NotNull(nameof(singleValueWriter), singleValueWriter);
            Guard.NotNull(nameof(summaryWriter), summaryWriter);
            
            _singleValueWriter = singleValueWriter;
            _summaryWriter = summaryWriter;
        }
        #endregion
        #region Methods
        /// <inheritdoc />
        public override Task MetricWriterSingleValueAsync(object userState, string name, string[] labels, double? value, CancellationToken cancellationToken)
        {
            return _singleValueWriter(userState, name, labels, value, cancellationToken);
        }

        /// <inheritdoc />
        public override Task MetricWriterSummaryAsync(object userState, string name, string[] labels, int? count, double? minimum, double? maximum, double? mean, double? standardDeviation, double? p50, double? p90, double? p95, double? p98, double? p99, CancellationToken cancellationToken)
        {
            return _summaryWriter(userState, name, labels, count, minimum, maximum, mean, standardDeviation, p50, p90, p95, p98, p99, cancellationToken);
        }
        #endregion
    }
}
