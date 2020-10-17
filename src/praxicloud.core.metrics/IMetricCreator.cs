// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    /// <summary>
    /// A type that has the ability to create metrics objects
    /// </summary>
    public interface IMetricCreator
    {
        #region Methods
        /// <summary>
        /// Creates a new counter with the specified name
        /// </summary>
        /// <param name="name">The display name of the counter</param>
        /// <param name="help">The help text associated with the counter</param>
        /// <param name="delayPublish">True if the counter should wait to publish information until it has a value assigned</param>
        /// <param name="labels">The labels associated with the counter</param>
        /// <returns>A counter instance</returns>
        ICounter CreateCounter(string name, string help, bool delayPublish, string[] labels);

        /// <summary>
        /// Creates a new gauge with the specified name
        /// </summary>
        /// <param name="name">The display name of the gauge</param>
        /// <param name="help">The help text associated with the gauge</param>
        /// <param name="delayPublish">True if the gauge should wait to publish information until it has a value assigned</param>
        /// <param name="labels">The labels associated with the gauge</param>
        /// <returns>A gauge instance</returns>
        IGauge CreateGauge(string name, string help, bool delayPublish, string[] labels);

        /// <summary>
        /// Creates a new summary with the specified name
        /// </summary>
        /// <param name="name">The display name of the summary</param>
        /// <param name="help">The help text assocaited with the summary</param>
        /// <param name="duration">The duration in seconds of each bucket (the aggregation is across this bucket)</param>
        /// <param name="delayPublish">True if the summary should wait to publish information until it has a value assigned</param>
        /// <param name="labels">The labels associated with the summary</param>
        /// <returns>A summary instance</returns>
        ISummary CreateSummary(string name, string help, long duration, bool delayPublish, string[] labels);

        /// <summary>
        /// Creates a new pulse with the specified name
        /// </summary>
        /// <param name="name">The display name of the pulse</param>
        /// <param name="help">The help text associated with the pulse</param>
        /// <param name="delayPublish">True if the summary should wait to publish information until it has a value assigned</param>
        /// <param name="labels">The labels associated with the pulse</param>
        /// <returns>A pulse instance</returns>
        IPulse CreatePulse(string name, string help, bool delayPublish, string[] labels);
        #endregion
    }
}
