// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.simpleprovider
{
    #region Using Clauses
    using System.Threading;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// A metric that can be notified to invoke a callback
    /// </summary>
    public interface ISimpleMetricWriter
    {
        #region Methods
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
