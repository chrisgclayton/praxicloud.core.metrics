// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    #region Using Clauses
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// A counter that increments in values and never decreases, only restarting when it is recreated
    /// </summary>
    public sealed class Counter : ICounter
    {
        #region Variables
        /// <summary>
        /// The list of counters that are controlled by this one aggregated counter
        /// </summary>
        private readonly ICounter[] _counters;
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="counters">The counters that this counter feeds</param>
        /// <param name="name">The name of the clounter</param>
        /// <param name="help">The help text associated with the counter</param>
        /// <param name="labels">The labels associated with the counter</param>
        internal Counter(ICounter[] counters, string name, string help, string[] labels)
        {            
            Name = name;
            Help = help;
            Labels = labels;

            _counters = counters;
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
            Parallel.ForEach(_counters, (counter) => counter.Increment());
        }

        /// <inheritdoc />
        public void IncrementBy(double value)
        {
            Parallel.ForEach(_counters, (counter) => counter.IncrementBy(value));
        }

        /// <inheritdoc />
        public void SetTo(double value)
        {
            Parallel.ForEach(_counters, (counter) => counter.SetTo(value));
        }
        #endregion
    }
}
