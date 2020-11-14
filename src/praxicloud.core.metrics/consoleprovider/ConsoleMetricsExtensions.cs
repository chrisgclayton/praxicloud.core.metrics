// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.consoleprovider
{
    /// <summary>
    /// An extension class for metric factories
    /// </summary>
    public static class ConsoleMetricsExtensions
    {
        /// <summary>
        /// Adds a console provider to the factory
        /// </summary>
        /// <param name="factory">The factory to add the console provider to</param>
        /// <param name="name">The user friendly and unique name of the provider</param>
        /// <param name="reportingInterval">The number of seconds before each call to counters to report values</param>
        /// <param name="includeLabels">True if the labels should be prefixed</param>
        /// <returns>The metric factory</returns>
        public static IMetricFactory AddConsole(this IMetricFactory factory, string name, long reportingInterval, bool includeLabels)
        {
            factory.AddProvider(name, new ConsoleMetricsProvider(reportingInterval, includeLabels));

            return factory;
        }
    }
}
