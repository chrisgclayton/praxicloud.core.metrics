// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    /// <summary>
    /// A factory used to create metrics
    /// </summary>
    public interface IMetricFactory : IMetricCreator
    {
        #region Methods
        /// <summary>
        /// Adds the metric provider to the factory where gauges, counters and summary instances created from the factory will write to all associated providers
        /// </summary>
        /// <param name="name">The name of the provider</param>
        /// <param name="provider">The provider to write to</param>
        void AddProvider(string name, IMetricProvider provider);
        #endregion
    }
}
