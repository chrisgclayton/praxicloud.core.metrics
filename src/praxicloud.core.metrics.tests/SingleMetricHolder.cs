// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.tests
{
    #region Using Clauses
    using System;
    using System.Diagnostics.CodeAnalysis;
    #endregion

    /// <summary>
    /// Container for a single metric value
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class SingleMetricHolder
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
        /// The last known value of the metric
        /// </summary>
        public double? Value { get; set; }
        #endregion
    }
}
