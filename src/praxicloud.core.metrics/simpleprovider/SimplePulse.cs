// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.simpleprovider
{
    #region using Clauses
    using System.Threading.Tasks;
    using System.Threading;
    #endregion

    /// <summary>
    /// A pulse that increments in values and restarts at each reporting interval, invoking a callback on each reporting period
    /// </summary>
    public sealed class SimplePulse : IPulse, ISimpleMetric
    {

        #region Variables
        /// <summary>
        /// The current value of the pulse
        /// </summary>
        private long _value = 0;

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
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="writer">The writer callback to be invoked when the output is to be written</param>
        /// <param name="userState">The user state to pass to the callback</param>
        /// <param name="name">The name of the pulse</param>
        /// <param name="delayPublish">Delay publishing of the pulse until a value is received</param>
        /// <param name="help">The help text associated with the pulse</param>
        /// <param name="labels">The labels assocaited with the pulse</param>
        public SimplePulse(ISimpleMetricWriter writer, object userState, bool delayPublish, string name, string help, string[] labels)
        {
            Name = name;
            Help = help;
            Labels = labels;

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
        /// <inheritdoc />
        public void Observe()
        {
            Interlocked.Increment(ref _value);
            _valueReceived = true;
        }

        /// <summary>
        /// Triggers the metric to invoke the metric
        /// </summary>
        public async Task WriteAsync(CancellationToken cancellationToken)
        {
            if (!_delayPublish || _valueReceived)
            {
                await _writer.MetricWriterSingleValueAsync(_userState, Name, Labels, Interlocked.Exchange(ref _value, 0), cancellationToken).ConfigureAwait(false);
            }
        }
        #endregion
    }
}
