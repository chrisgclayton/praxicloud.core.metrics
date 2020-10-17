// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.tests
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using praxicloud.core.metrics.callbackprovider;
    using praxicloud.core.metrics.consoleprovider;
    using praxicloud.core.metrics.debugprovider;
    using praxicloud.core.metrics.traceprovider;
    using praxicloud.core.security;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;
    #endregion

    /// <summary>
    /// A set of tests to metric provider functionality
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ProviderTests
    {
        /// <summary>
        /// Validates a non disposable provider will clean up appropriately
        /// </summary>
        [TestMethod]
        public void NonDisposable()
        {
            using(var factory = new MetricFactory())
            {
                factory.AddProvider("test1", new NonDisposableProvider());
                factory.AddProvider("test2", new NonDisposableProvider());
            }
        }

        /// <summary>
        /// Validates a non disposable provider will clean up appropriately
        /// </summary>
        [TestMethod]
        public void NonDisposable2()
        {
            using (var factory = new MetricFactory())
            {
                factory.AddProvider("test1", (IMetricProvider)new NonDisposableProvider());
                factory.AddProvider("test2", (IMetricProvider)new NonDisposableProvider());
            }
        }

        /// <summary>
        /// Validates replacement of a named provider
        /// </summary>
        [TestMethod]
        public void ReplaceNamedProvider()
        {
            using (var factory = new MetricFactory())
            {
                factory.AddProvider("test1", (IMetricProvider)new NonDisposableProvider());
                factory.AddProvider("test1", (IMetricProvider)new NonDisposableProvider());
            }
        }

        /// <summary>
        /// Validates a non disposable provider will clean up appropriately
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void NullProviderException()
        {
            using (var factory = new MetricFactory())
            {
                factory.AddProvider("test1", null);
            }
        }

        /// <summary>
        /// A non disposable metric provider
        /// </summary>
        public class NonDisposableProvider : IMetricProvider
        {           
            /// <inheritdoc />
            public ICounter CreateCounter(string name, string help, bool delayPublish, string[] labels)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public IGauge CreateGauge(string name, string help, bool delayPublish, string[] labels)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public IPulse CreatePulse(string name, string help, bool delayPublish, string[] labels)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public ISummary CreateSummary(string name, string help, long duration, bool delayPublish, string[] labels)
            {
                throw new NotImplementedException();
            }
        }
    }
}
