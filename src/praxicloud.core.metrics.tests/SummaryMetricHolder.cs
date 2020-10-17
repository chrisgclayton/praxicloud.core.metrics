// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.tests
{
    #region Using Clauses
    using System;
    using System.Diagnostics.CodeAnalysis;
    #endregion

    /// <summary>
    /// Container for a summary metric value
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class SummaryMetricHolder
    {
        #region Properties
        /// <summary>
        /// The time the sample was presented at
        /// </summary>
        public DateTime SampleTime { get; set; }

        /// <summary>
        /// User state data passed into the metric
        /// </summary>
        public object UserState { get; set; }

        /// <summary>
        /// The name of the metric being tracked
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The labels associated with the metric
        /// </summary>
        public string[] Labels { get; set; }

        /// <summary>
        /// The count of samples
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// The minimum value
        /// </summary>
        public double? Minimum { get; set; }

        /// <summary>
        /// The maximum value
        /// </summary>
        public double? Maximum { get; set; }

        /// <summary>
        /// The mean of the values
        /// </summary>
        public double? Mean { get; set; }

        /// <summary>
        /// The standard deviation of the values
        /// </summary>
        public double? StandardDeviation { get; set; }

        /// <summary>
        /// The 50th percentile
        /// </summary>
        public double? p50 { get; set; }

        /// <summary>
        /// The 90th percentile
        /// </summary>
        public double? p90 { get; set; }

        /// <summary>
        /// The 95th percentile
        /// </summary>
        public double? p95 { get; set; }

        /// <summary>
        /// The 98th percentile
        /// </summary>
        public double? p98 { get; set; }

        /// <summary>
        /// The 99th percentile
        /// </summary>
        public double? p99 { get; set; }
        #endregion
    }
}
