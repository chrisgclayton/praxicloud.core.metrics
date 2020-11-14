// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.tests
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics.CodeAnalysis;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class MetricUtilitiesTest
    {
        /// <summary>
        /// Creates the name of metrics
        /// </summary>
        [DataTestMethod]
        [DataRow("prefix", "metric", "unitName", "aggregate")]
        [DataRow(null, "metric", "unitName", "aggregate")]
        [DataRow("prefix", null, "unitName", "aggregate")]
        [DataRow("prefix", "metric", null, "aggregate")]
        [DataRow("prefix", "metric", "unitName", null)]
        [DataRow(null, "metric", "unitName", null)]
        [DataRow(null, "metric", null, "aggregate")]
        [DataRow(null, null, "unitName", "aggregate")]
        [DataRow("prefix", null, null, "aggregate")]
        [DataRow("prefix", null, "unitName", null)]
        [DataRow("prefix", "metric", null, null)]
        [DataRow(null, null, null, "aggregate")]
        [DataRow(null, null, "unitName", null)]
        [DataRow(null, "metric", null, null)]
        [DataRow("prefix", null, null, null)]
        [DataRow("prefix-test", "metric name", "unitName spaced", "aggregate out")]
        [DataRow(" prefix ", " metric ", " unitName ", " aggregate ")]
        [DataRow("p;refix", "me;tric", "uni'tName", "ag'gregate")]
        [DataRow("prefix", "metric", "unitName", "aggregate")]
        [DataRow(" ", "metric", "unitName", "aggregate")]
        [DataRow("prefix", " ", "unitName", "aggregate")]
        [DataRow("prefix", "metric", " ", "aggregate")]
        [DataRow("prefix", "metric", "unitName", " ")]
        [DataRow(" ", "metric", "unitName", " ")]
        [DataRow(" ", "metric", " ", "aggregate")]
        [DataRow(" ", " ", "unitName", "aggregate")]
        [DataRow("prefix", " ", " ", "aggregate")]
        [DataRow("prefix", " ", "unitName", " ")]
        [DataRow("prefix", "metric", " ", " ")]
        [DataRow(" ", " ", " ", "aggregate")]
        [DataRow(" ", " ", "unitName", " ")]
        [DataRow(" ", "metric", " ", " ")]
        [DataRow("prefix", " ", " ", " ")]
        [DataRow("", "metric", "unitName", "aggregate")]
        [DataRow("prefix", "", "unitName", "aggregate")]
        [DataRow("prefix", "metric", "", "aggregate")]
        [DataRow("prefix", "metric", "unitName", "")]
        [DataRow("", "metric", "unitName", "")]
        [DataRow("", "metric", "", "aggregate")]
        [DataRow("", "", "unitName", "aggregate")]
        [DataRow("prefix", "", "", "aggregate")]
        [DataRow("prefix", "", "unitName", "")]
        [DataRow("prefix", "metric", "", "")]
        [DataRow("", "", "", "aggregate")]
        [DataRow("", "", "unitName", "")]
        [DataRow("", "metric", "", "")]
        [DataRow("prefix", "", "", "")]
        public void MetricNameCreation(string applicationPrefix, string metric, string unitName, string aggregate)
        {
            string name = MetricUtilities.CreateMetricName(applicationPrefix, metric, unitName, aggregate); 


            string cleanMetricName = applicationPrefix?.Trim().Replace(" ", "").ToLowerInvariant() ?? ""; 
            string cleanMetric = metric?.Trim().Replace(" ", "").ToLowerInvariant(); 

            if(!string.IsNullOrWhiteSpace(cleanMetric))
            {
                if(!string.IsNullOrWhiteSpace(cleanMetricName))
                {
                    cleanMetricName += "_";
                }

                cleanMetricName += cleanMetric;
            }

            string cleanUnitName = unitName?.Trim().Replace(" ", "").ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(cleanUnitName))
            {
                if (!string.IsNullOrWhiteSpace(cleanMetricName))
                {
                    cleanMetricName += "_";
                }

                cleanMetricName += cleanUnitName;
            }

            string cleanAggregate = aggregate?.Trim().Replace(" ", "").ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(cleanAggregate))
            {
                if (!string.IsNullOrWhiteSpace(cleanMetricName))
                {
                    cleanMetricName += "_";
                }

                cleanMetricName += cleanAggregate;
            }

            Assert.IsTrue(string.Equals(name, cleanMetricName, System.StringComparison.Ordinal), "Metric name not expected");
        }


        /// <summary>
        /// Creates the name of metrics
        /// </summary>
        [DataTestMethod]
        [DataRow("prefix", "metric", "unitName")]
        [DataRow(null, "metric", "unitName")]
        [DataRow("prefix", null, "unitName")]
        [DataRow("prefix", "metric", null)]
        [DataRow(null, "metric", null)]
        [DataRow(null, null, "unitName")]
        [DataRow("prefix", null, null)]
        [DataRow(null, null, null)]
        [DataRow("prefix-test", "metric name", "unitName spaced")]
        [DataRow(" prefix ", " metric ", " unitName ")]
        [DataRow("p;refix", "me;tric", "uni'tName")]
        [DataRow(" ", "metric", "unitName")]
        [DataRow("prefix", " ", "unitName")]
        [DataRow("prefix", "metric", " ")]
        [DataRow(" ", "metric", " ")]
        [DataRow(" ", " ", "unitName")]
        [DataRow("prefix", " ", " ")]
        public void MetricNameCreation(string applicationPrefix, string metric, string unitName)
        {
            string name = MetricUtilities.CreateMetricName(applicationPrefix, metric, unitName);


            string cleanMetricName = applicationPrefix?.Trim().Replace(" ", "").ToLowerInvariant() ?? "";
            string cleanMetric = metric?.Trim().Replace(" ", "").ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(cleanMetric))
            {
                if (!string.IsNullOrWhiteSpace(cleanMetricName))
                {
                    cleanMetricName += "_";
                }

                cleanMetricName += cleanMetric;
            }

            string cleanUnitName = unitName?.Trim().Replace(" ", "").ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(cleanUnitName))
            {
                if (!string.IsNullOrWhiteSpace(cleanMetricName))
                {
                    cleanMetricName += "_";
                }

                cleanMetricName += cleanUnitName;
            }

            Assert.IsTrue(string.Equals(name, cleanMetricName, System.StringComparison.Ordinal), "Metric name not expected");
        }


        /// <summary>
        /// Creates the name of metrics
        /// </summary>
        [DataTestMethod]
        [DataRow("prefix", "metric")]
        [DataRow(null, "metric")]
        [DataRow("prefix", null)]
        [DataRow(null, null)]
        public void MetricNameCreation(string applicationPrefix, string metric)
        {
            string name = MetricUtilities.CreateMetricName(applicationPrefix, metric);


            string cleanMetricName = applicationPrefix?.Trim().Replace(" ", "").ToLowerInvariant() ?? "";
            string cleanMetric = metric?.Trim().Replace(" ", "").ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(cleanMetric))
            {
                if (!string.IsNullOrWhiteSpace(cleanMetricName))
                {
                    cleanMetricName += "_";
                }

                cleanMetricName += cleanMetric;
            }

            Assert.IsTrue(string.Equals(name, cleanMetricName, System.StringComparison.Ordinal), "Metric name not expected");
        }
    }
}
