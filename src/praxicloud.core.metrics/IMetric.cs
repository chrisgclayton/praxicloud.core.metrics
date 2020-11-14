// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    /// <summary>
    /// The core metric implementation
    /// </summary>
    public interface IMetric
    {
        #region Properties
        /// <summary>
        /// The name of the metric
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The help text associated with the metric
        /// </summary>
        string Help { get; }

        /// <summary>
        /// Labels associated with the metric that may demonstrate different views. It is up to the metric provider whether it is supported and the number of labels that are supported, no exceptions will be raised as a result but 
        /// many providers will demonstrate a performance impact for too many labels as it may create descrete metrics.
        /// </summary>
        string[] Labels { get; }
        #endregion
    }
}
