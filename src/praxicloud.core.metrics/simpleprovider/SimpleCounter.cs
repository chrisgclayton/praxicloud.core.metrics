// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.simpleprovider
{
    #region using Clauses
    using System.Threading.Tasks;
    using System.Threading;
    #endregion

    /// <summary>
    /// A counter that increments in values and never decreases, only restarting when it is recreated, invoking a callback on each reporting period
    /// </summary>
    public sealed class SimpleCounter : ICounter, ISimpleMetric
    {

        #region Variables
        /// <summary>
        /// The current value of the counter
        /// </summary>
        private double _value = 0;

        /// <summary>
        /// A control used to ensure accurate updates
        /// </summary>
        private readonly object _control = new object();

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
        /// <param name="name">The name of the counter</param>
        /// <param name="delayPublish">Delay publishing of the counter until a value is received</param>
        /// <param name="help">The help text associated with the counter</param>
        /// <param name="labels">The labels assocaited with the counter</param>
        public SimpleCounter(ISimpleMetricWriter writer, object userState, bool delayPublish, string name, string help, string[] labels)
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
        public void Increment()
        {
            IncrementBy(1.0);
        }

        /// <inheritdoc />
        public void IncrementBy(double value)
        {
            lock (_control)
            {
                _value += value;
            }

            _valueReceived = true;
        }

        /// <inheritdoc />
        public void SetTo(double value)
        {
            lock (_control)
            {
                _value = value;
            }

            _valueReceived = true;
        }

        /// <summary>
        /// Triggers the metric to invoke the metric
        /// </summary>
        public async Task WriteAsync(CancellationToken cancellationToken)
        {
            if(!_delayPublish || _valueReceived) await _writer.MetricWriterSingleValueAsync(_userState, Name, Labels, _value, cancellationToken).ConfigureAwait(false);
        }
        #endregion
    }
}
