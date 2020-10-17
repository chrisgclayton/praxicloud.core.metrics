// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.debugprovider
{
    /// <summary>
    /// An extension class for metric factories
    /// </summary>
    public static class DebugMetricsExtensions
    {
        /// <summary>
        /// Adds a debug provider to the factory
        /// </summary>
        /// <param name="factory">The factory to add the debug provider to</param>
        /// <param name="name">The user friendly and unique name of the provider</param>
        /// <param name="reportingInterval">The number of seconds before each call to counters to report values</param>
        /// <param name="includeLabels">True if the labels should be prefixed</param>
        /// <returns>The metric factory</returns>
        public static IMetricFactory AddDebug(this IMetricFactory factory, string name, long reportingInterval, bool includeLabels)
        {
            factory.AddProvider(name, new DebugMetricsProvider(reportingInterval, includeLabels));

            return factory;
        }
    }
}
