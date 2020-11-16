// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    #region Using Clauses
    using System;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// A gauge that increments in values and can decrease.
    /// </summary>
    public sealed class Gauge : IGauge
    {
        #region Variables
        /// <summary>
        /// The list of counters that are controlled by this one aggregated counter
        /// </summary>
        private readonly IGauge[] _gauges;
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="gauges">The gauges that this counter feeds</param>
        /// <param name="name">The name of the clounter</param>
        /// <param name="help">The help text associated with the counter</param>
        /// <param name="labels">The labels associated with the counter</param>
        internal Gauge(IGauge[] gauges, string name, string help, string[] labels)
        {
            Name = name;
            Help = help;
            Labels = labels;

            _gauges = gauges;
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
        public void Decrement()
        {
            Parallel.ForEach(_gauges, (gauge) => gauge.Decrement());
        }

        /// <inheritdoc />
        public void DecrementBy(double value)
        {
            Parallel.ForEach(_gauges, (gauge) => gauge.DecrementBy(value));
        }

        /// <inheritdoc />
        public void Increment()
        {
            Parallel.ForEach(_gauges, (gauge) => gauge.Increment());
        }

        /// <inheritdoc />
        public void IncrementBy(double value)
        {
            Parallel.ForEach(_gauges, (gauge) => gauge.IncrementBy(value));
        }

        /// <inheritdoc />
        public void SetTo(double value)
        {
            Parallel.ForEach(_gauges, (gauge) => gauge.SetTo(value));
        }

        /// <inheritdoc />
        public IDisposable TrackExecution()
        {
            var trackResults = new IDisposable[_gauges.Length];
     
            for(var index = 0; index < trackResults.Length; index++)
            {
                trackResults[index] = _gauges[index].TrackExecution();
            }

            return new Tracker(trackResults);
        }
        #endregion
        #region Execution Tracking Type
        /// <summary>
        /// An type that tracks aggregates multiple disposable items
        /// </summary>
        private sealed class Tracker : IDisposable
        {
            #region Variables
            /// <summary>
            /// The disposable objects to dispose of when this object is disposed
            /// </summary>
            private readonly IDisposable[] _toDisposeOf;
            #endregion
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the type
            /// </summary>
            /// <param name="toDisposeOf">The disposable objects to dispose of when this object is disposed</param>
            public Tracker(IDisposable[] toDisposeOf)
            {
                _toDisposeOf = toDisposeOf;
            }
            #endregion
            #region Methods
            /// <inheritdoc />
            public void Dispose()
            {
                Parallel.ForEach(_toDisposeOf, (toDisposeOf) => toDisposeOf.Dispose());
            }
            #endregion
        }
        #endregion
    }
}
