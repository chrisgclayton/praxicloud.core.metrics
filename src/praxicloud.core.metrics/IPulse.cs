// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    #region Using Clauses
    using System;
    #endregion

    /// <summary>
    /// A metric that indicates an known event occurred
    /// </summary>
    public interface IPulse : IMetric
    {
        #region Methods
        /// <summary>
        /// Records that an event occurred
        /// </summary>
        void Observe();
        #endregion
    }
}
