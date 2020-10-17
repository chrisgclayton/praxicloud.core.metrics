// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    /// <summary>
    /// A metric that increases in value, never decreasing except when restarted
    /// </summary>
    public interface ICounter : IMetric
    {
        #region Methods
        /// <summary>
        /// Increments the counter by 1.0
        /// </summary>
        void Increment();

        /// <summary>
        /// Increments the current value by values
        /// </summary>
        /// <param name="value">The amount to increase the metric by</param>
        void IncrementBy(double value);

        /// <summary>
        /// Sets the value to the specified value as long as it is greater than the current value
        /// </summary>
        /// <param name="value">The value to set the counter to</param>
        void SetTo(double value);
        #endregion
    }
}
