// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.debugprovider
{
    #region Using Clauses
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using praxicloud.core.metrics.simpleprovider;
    #endregion

    /// <summary>
    /// A metrics provider that outputs to the console
    /// </summary>
    public sealed class DebugMetricsProvider : SimpleMetricsProvider
    {
        #region Variables
        /// <summary>
        /// True if the writer should output the label prefix
        /// </summary>
        private readonly bool _includeLabels;

        /// <summary>
        /// A dictionary of labels for each metric
        /// </summary>
        private readonly Dictionary<string, string> _metricLabels = new Dictionary<string, string>();
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="reportingInterval">The number of seconds before each call to counters to report values</param>
        /// <param name="includeLabels">True if the labels should be prefixed</param>
        public DebugMetricsProvider(long reportingInterval, bool includeLabels) : base(reportingInterval, null)
        {
            _includeLabels = includeLabels;
        }
        #endregion
        #region Methods
        /// <inheritdoc />
        protected override void MetricPrecreate(string name, string help, bool delayPublish, string[] labels)
        {
            _metricLabels.Add(name, TextOutputUtilities.GetLabelText(_includeLabels, labels));
        }

        /// <summary>
        /// Writes a metric
        /// </summary>
        /// <param name="name">The name of the metric</param>
        /// <param name="userState">A user state object that was provided to the provider</param>
        /// <param name="labels">The labels associated with the metric</param>
        /// <param name="value">The value that the metric is currently at</param>
        /// <param name="cancellationToken">A token to monitor for abort requests</param>
        public override Task MetricWriterSingleValueAsync(object userState, string name, string[] labels, double? value, CancellationToken cancellationToken)
        {
            return Task.Run(() => Debug.Print($"{ _metricLabels[name] }{ name } = { TextOutputUtilities.GetValueText(value) }"));
        }

        /// <summary>
        /// Writes a summary
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
        public override Task MetricWriterSummaryAsync(object userState, string name, string[] labels, int? count, double? minimum, double? maximum, double? mean, double? standardDeviation, double? p50, double? p90, double? p95, double? p98, double? p99, CancellationToken cancellationToken)
        {
            return Task.Run(() => Debug.Print($"{ _metricLabels[name] }{ name } = ( count = { TextOutputUtilities.GetValueText(count) }, min = { TextOutputUtilities.GetValueText(minimum) }, max = { TextOutputUtilities.GetValueText(maximum) }, mean = { TextOutputUtilities.GetValueText(mean) }, std = { TextOutputUtilities.GetValueText(standardDeviation) }, p50 = { TextOutputUtilities.GetValueText(p50) }, p90 = { TextOutputUtilities.GetValueText(p90) }, p95 = { TextOutputUtilities.GetValueText(p95) }, p98 = { TextOutputUtilities.GetValueText(p98) }, p99 = { TextOutputUtilities.GetValueText(p99) } )"));
        }
        #endregion
    }
}
