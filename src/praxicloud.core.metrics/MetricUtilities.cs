// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    #region Using Clauses
    using System.Text;
    #endregion

    /// <summary>
    /// Helper utilities for working with metrics
    /// </summary>
    public static class MetricUtilities
    {
        /// <summary>
        /// Creates a metric name following the recommended format
        /// </summary>
        /// <param name="applicationPrefix">A prefix indicating the name of the application or library</param>
        /// <param name="metric">The metric being recorded</param>
        /// <param name="unitName">The name of the unit being recorded</param>
        /// <param name="aggregate">The aggregate that is in use</param>
        /// <returns>A string representation of the metric name following recommended formatting rules</returns>
        public static string CreateMetricName(string applicationPrefix, string metric, string unitName, string aggregate)
        {
            return CreateMetricNameInternal(new StringBuilder(), applicationPrefix, metric, unitName, aggregate).ToString();
        }

        /// <summary>
        /// Creates a metric name following the recommended format
        /// </summary>
        /// <param name="applicationPrefix">A prefix indicating the name of the application or library</param>
        /// <param name="metric">The metric being recorded</param>
        /// <param name="unitName">The name of the unit being recorded</param>
        /// <returns>A string representation of the metric name following recommended formatting rules</returns>
        public static string CreateMetricName(string applicationPrefix, string metric, string unitName)
        {
            return CreateMetricNameInternal(new StringBuilder(), applicationPrefix, metric, unitName).ToString();
        }

        /// <summary>
        /// Creates a metric name following the recommended format
        /// </summary>
        /// <param name="applicationPrefix">A prefix indicating the name of the application or library</param>
        /// <param name="metric">The metric being recorded</param>
        /// <returns>A string representation of the metric name following recommended formatting rules</returns>
        public static string CreateMetricName(string applicationPrefix, string metric)
        {
            return CreateMetricNameInternal(new StringBuilder(), applicationPrefix, metric).ToString();
        }

        /// <summary>
        /// Creates a metric name following the recommended format
        /// </summary>
        /// <param name="applicationPrefix">A prefix indicating the name of the application or library</param>
        /// <param name="metric">The metric being recorded</param>
        /// <param name="builder">The string builder to write the name to</param>
        /// <returns>A string builder that has the portion of the metric name</returns>
        private static StringBuilder CreateMetricNameInternal(StringBuilder builder, string applicationPrefix, string metric)
        {
            if (!string.IsNullOrWhiteSpace(applicationPrefix))
            {
                if (builder.Length > 0) builder.Append("_");
                builder.Append(CleanMetricElement(applicationPrefix));
            }

            if (!string.IsNullOrWhiteSpace(metric))
            {
                if (builder.Length > 0) builder.Append("_");
                builder.Append(CleanMetricElement(metric));
            }

            return builder;
        }

        /// <summary>
        /// Creates a metric name following the recommended format
        /// </summary>
        /// <param name="applicationPrefix">A prefix indicating the name of the application or library</param>
        /// <param name="metric">The metric being recorded</param>
        /// <param name="builder">The string builder to write the name to</param>
        /// <param name="unitName">The name of the unit being recorded</param> 
        /// <returns>A string builder that has the portion of the metric name</returns>
        private static StringBuilder CreateMetricNameInternal(StringBuilder builder, string applicationPrefix, string metric, string unitName)
        {
            builder = CreateMetricNameInternal(builder, applicationPrefix, metric);

            if (!string.IsNullOrWhiteSpace(unitName))
            {
                if (builder.Length > 0) builder.Append("_");
                builder.Append(CleanMetricElement(unitName));
            }

            return builder;
        }

        /// <summary>
        /// Creates a metric name following the recommended format
        /// </summary>
        /// <param name="applicationPrefix">A prefix indicating the name of the application or library</param>
        /// <param name="metric">The metric being recorded</param>
        /// <param name="builder">The string builder to write the name to</param>
        /// <param name="unitName">The name of the unit being recorded</param> 
        /// <param name="aggregate">The aggregate that is in use</param>
        /// <returns>A string builder that has the portion of the metric name</returns>
        private static StringBuilder CreateMetricNameInternal(StringBuilder builder, string applicationPrefix, string metric, string unitName, string aggregate)
        {
            builder = CreateMetricNameInternal(builder, applicationPrefix, metric, unitName);

            if (!string.IsNullOrWhiteSpace(aggregate))
            {
                if (builder.Length > 0) builder.Append("_");
                builder.Append(CleanMetricElement(aggregate));
            }

            return builder;
        }

        /// <summary>
        /// Cleans the metric element to remove whitespace
        /// </summary>
        /// <param name="element">The element to clean</param>
        /// <returns>A string version of the value</returns>
        private static string CleanMetricElement(string element)
        {        
            return element.Trim().Replace(" ", "").ToLowerInvariant();
        }
    }
}
