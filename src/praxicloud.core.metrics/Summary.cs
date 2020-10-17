// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    #region Using Clauses
    using System;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// Summarizes the value over a period of time
    /// </summary>
    public sealed class Summary : ISummary
    {
        #region Variables
        /// <summary>
        /// The list of summary that are controlled by this one aggregated counter
        /// </summary>
        private readonly ISummary[] _summaries;
        #endregion
        #region Properties
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Help { get; }

        /// <inheritdoc />
        public string[] Labels { get; }
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="summaries">The summaries that this counter feeds</param>
        /// <param name="name">The name of the summary</param>
        /// <param name="help">The help text associated with the summary</param>
        /// <param name="labels">The labels associated with the summary</param>
        internal Summary(ISummary[] summaries, string name, string help, string[] labels)
        {
            Name = name;
            Help = help;
            Labels = labels;

            _summaries = summaries;
        }
        #endregion
        #region Methods
        /// <inheritdoc />
        public void Observe(double value)
        {
            Parallel.ForEach(_summaries, (summary) => summary.Observe(value));
        }

        /// <inheritdoc />
        public IDisposable Time()
        {
            var trackResults = new IDisposable[_summaries.Length];

            for(var index = 0; index < trackResults.Length; index++)
            {
                trackResults[index] = trackResults[index] = _summaries[index].Time();
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
