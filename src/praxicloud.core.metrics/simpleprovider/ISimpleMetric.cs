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
    public interface ISimpleMetric
    {
        /// <summary>
        /// Write the information to the metric sink
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for abort requests</param>
        Task WriteAsync(CancellationToken cancellationToken);
    }
}
