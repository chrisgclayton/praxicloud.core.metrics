// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    #region Using Clauses
    using System;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// Indicates that a known event occurred
    /// </summary>
    public sealed class Pulse : IPulse
    {
        #region Variables
        /// <summary>
        /// The list of pulses that are controlled by this one aggregated counter
        /// </summary>
        private readonly IPulse[] _pulses;
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
        /// <param name="pulses">The pulses that this counter feeds</param>
        /// <param name="name">The name of the pulse</param>
        /// <param name="help">The help text associated with the pulse</param>
        /// <param name="labels">The labels associated with the pulse</param>
        internal Pulse(IPulse[] pulses, string name, string help, string[] labels)
        {
            Name = name;
            Help = help;
            Labels = labels;

            _pulses = pulses;
        }
        #endregion
        #region Methods
        /// <inheritdoc />
        public void Observe()
        {
            Parallel.ForEach(_pulses, (pulse) => pulse.Observe());
        }
        #endregion
    }
}
