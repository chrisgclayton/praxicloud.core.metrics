// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.simpleprovider
{
    #region using Clauses
    using System.Threading.Tasks;
    using System;
    using System.Diagnostics;
    using System.Threading;
    #endregion

    /// <summary>
    /// A summary that increments in values and never decreases, only restarting when it is recreated, invoking a callback on each reporting period
    /// </summary>
    public sealed class SimpleSummary : ISummary, ISimpleMetric
    {
        #region Variables
        /// <summary>
        /// The metric writer to invoke when writing is to be performed
        /// </summary>
        private readonly ISimpleMetricWriter _writer;

        /// <summary>
        /// The user state to pass to the callback when invoking
        /// </summary>
        private readonly object _userState;

        /// <summary>
        /// True if the metric should delay publishing until a valid value is received
        /// </summary>
        private readonly bool _delayPublish;

        /// <summary>
        /// True when a value has been written to the metric
        /// </summary>
        private bool _valueReceived = false;

        /// <summary>
        /// The bucket used for aggregation
        /// </summary>
        private readonly MetricBucket _bucket;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="writer">The writer callback to be invoked when the output is to be written</param>
        /// <param name="userState">The user state to pass to the callback</param>
        /// <param name="name">The name of the counter</param>
        /// <param name="delayPublish">Delay publishing of the counter until a value is received</param>
        /// <param name="help">The help text associated with the counter</param>
        /// <param name="labels">The labels assocaited with the counter</param>
        /// <param name="bucketDuration">The number of seconds in each bucket interval</param>
        public SimpleSummary(ISimpleMetricWriter writer, object userState, bool delayPublish, long bucketDuration, string name, string help, string[] labels)
        {
            Name = name;
            Help = help;
            Labels = labels;

            _bucket = new MetricBucket(bucketDuration);
            _delayPublish = delayPublish;
            _userState = userState;
            _writer = writer;
        }
        #endregion
        #region Properties
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Help { get; }

        /// <inheritdoc />
        public string[] Labels { get; }
        #endregion
        #region Methods
        /// <summary>
        /// Triggers the metric to invoke the metric
        /// </summary>
        public async Task WriteAsync(CancellationToken cancellationToken)
        {
            if (!_delayPublish || _valueReceived)
            {
                _bucket.GetAggregates(out var startTime, out var count, out var maximum, out var minimum, out var mean, out var standardDeviation, out var p50, out var p90, out var p95, out var p98, out var p99);

                if (startTime != null && count != null)
                {
                    await _writer.MetricWriterSummaryAsync(_userState, Name, Labels, count, minimum, maximum, mean, standardDeviation, p50, p90, p95, p98, p99, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <inheritdoc />
        public IDisposable Time()
        {
            return new Tracker(this);
        }

        /// <inheritdoc />
        public void Observe(double value)
        {
            _bucket.AddValue(DateTimeOffset.UtcNow, value);
            _valueReceived = true;
        }
        #endregion
        #region Execution Tracking Type
        /// <summary>
        /// An type that writes the time from instantiation to disposal in a summary observed value
        /// </summary>
        private sealed class Tracker : IDisposable
        {
            #region Variables
            /// <summary>
            /// The summary to track with
            /// </summary>
            private readonly ISummary _summary;

            /// <summary>
            /// The stopwatch to track with
            /// </summary>
            private readonly Stopwatch _watch;
            #endregion
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the type
            /// </summary>
            /// <param name="summary">The summary to modify</param>
            public Tracker(ISummary summary)
            {
                _summary = summary;

                _watch = Stopwatch.StartNew();
            }
            #endregion
            #region Methods
            /// <inheritdoc />
            public void Dispose()
            {
                _watch.Stop();
                _summary.Observe(_watch.ElapsedMilliseconds);
            }
            #endregion
        }
        #endregion

    }
}
