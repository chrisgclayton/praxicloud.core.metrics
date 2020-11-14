// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    #region Using Clauses
    using System;
    #endregion

    /// <summary>
    /// A metric that increases and decreases
    /// </summary>
    public interface IGauge : IMetric
    {
        #region Methods
        /// <summary>
        /// Increments the gauge by 1.0
        /// </summary>
        void Increment();

        /// <summary>
        /// Incrememnt the current value by the specified value
        /// </summary>
        /// <param name="value">The value to increment the value by</param>
        void IncrementBy(double value);

        /// <summary>
        /// Decrements the gauge by 1.0
        /// </summary>
        void Decrement();

        /// <summary>
        /// Decrements the current value by the specified value
        /// </summary>
        /// <param name="value">The value to decrement the value by</param>
        void DecrementBy(double value);

        /// <summary>
        /// Sets the value to the specified value as long as it is greater than the current value
        /// </summary>
        /// <param name="value">The value to set the counter to</param>
        void SetTo(double value);

        /// <summary>
        /// Creates a concurrency tracker that increments the counter when instantiated and decrememnts when disposed of
        /// </summary>
        /// <returns>An instance of a disposable object htat when disposed decrements the metric</returns>
        IDisposable TrackExecution();
        #endregion
    }
}
