// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.callbackprovider
{
    /// <summary>
    /// An extension class for metric factories
    /// </summary>
    public static class CallbackMetricsExtensions
    {
        /// <summary>
        /// Adds a callback provider to the factory
        /// </summary>
        /// <param name="factory">The factory to add the callback provider to</param>
        /// <param name="name">The user friendly and unique name of the provider</param>
        /// <param name="reportingInterval">The number of seconds before each call to counters to report values</param>
        /// <param name="singleValueWriter">The callback method that is invoked for single value writes</param>
        /// <param name="summaryWriter">The callback method that is invoked for summary data writes</param>
        /// <param name="userState">A user state object that consumers can provide that is meaningful to their callbacks</param>
        /// <returns>The metric factory</returns>
        public static IMetricFactory AddCallback(this IMetricFactory factory, string name, long reportingInterval, CallbackMetricsProvider.MetricWriterSingleValue singleValueWriter, CallbackMetricsProvider.MetricWriterSummary summaryWriter, object userState)
        {
            factory.AddProvider(name, new CallbackMetricsProvider(reportingInterval, singleValueWriter, summaryWriter, userState));

            return factory;
        }
    }
}
