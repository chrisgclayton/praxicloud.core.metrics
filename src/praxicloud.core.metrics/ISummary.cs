// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    #region Using Clauses
    using System;
    #endregion

    /// <summary>
    /// Summarizes the value over a period of time
    /// </summary>
    public interface ISummary : IMetric
    {
        #region Methods
        /// <summary>
        /// Times the time between calling Time() and diposing of the returned IDisposable object
        /// </summary>
        /// <returns>An instance that should be disposed of to complete the timing entry</returns>
        IDisposable Time();

        /// <summary>
        /// Adds an observed value for the current UTC time
        /// </summary>
        /// <param name="value">The value that was observed</param>
        void Observe(double value);
        #endregion
    }
}
