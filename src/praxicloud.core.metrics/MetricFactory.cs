// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    #region Using Clauses
    using praxicloud.core.security;
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Runtime.CompilerServices;
    #endregion

    /// <summary>
    /// A factory instance that creates metrics based on the provider list
    /// </summary>
    public class MetricFactory : IMetricFactory, IDisposable
    {
        #region Variables
        /// <summary>
        /// A list of providers that the factory associates with each counter it creates
        /// </summary>
        private readonly ConcurrentDictionary<string, IMetricProvider> _providers = new ConcurrentDictionary<string, IMetricProvider>();
        #endregion
        #region Methods
        /// <inheritdoc />
        public void AddProvider(string name, IMetricProvider provider)
        {
            Guard.NotNullOrWhitespace(nameof(name), name);
            Guard.NotNull(nameof(provider), provider);

            _providers.AddOrUpdate(name, provider, (currentName, currentProvider) => provider);
        }

        /// <inheritdoc />
        public ICounter CreateCounter(string name, string help, bool delayPublish, string[] labels)
        {
            var counters = new ICounter[_providers.Count];

            for(var index = 0; index < counters.Length; index++)
            {
                counters[index] = _providers.ElementAt(index).Value.CreateCounter(name, help, delayPublish, labels);
            }

            return counters.Length == 1 ? counters[0] : new Counter(counters, name, help, labels);
        }

        /// <inheritdoc />
        public IGauge CreateGauge(string name, string help, bool delayPublish, string[] labels)
        {
            var gauges = new IGauge[_providers.Count];

            for (var index = 0; index < gauges.Length; index++)
            {
                gauges[index] = _providers.ElementAt(index).Value.CreateGauge(name, help, delayPublish, labels);
            }

            return gauges.Length == 1 ? gauges[0] : new Gauge(gauges, name, help, labels);
        }

        /// <inheritdoc />
        public IPulse CreatePulse(string name, string help, bool delayPublish, string[] labels)
        {
            var pulses = new IPulse[_providers.Count];

            for (var index = 0; index < pulses.Length; index++)
            {
                pulses[index] = _providers.ElementAt(index).Value.CreatePulse(name, help, delayPublish, labels);
            }

            return pulses.Length == 1 ? pulses[0] : new Pulse(pulses, name, help, labels);
        }

        /// <inheritdoc />
        public ISummary CreateSummary(string name, string help, long duration, bool delayPublish, string[] labels)
        {
            var summaries = new ISummary[_providers.Count];

            for (var index = 0; index < summaries.Length; index++)
            {
                summaries[index] = _providers.ElementAt(index).Value.CreateSummary(name, help, duration, delayPublish, labels);
            }

            return summaries.Length == 1 ? summaries[0] : new Summary(summaries, name, help, labels);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach(var pair in _providers)
            {
                (pair.Value as IDisposable)?.Dispose();
            }
        }
        #endregion
    }
}
