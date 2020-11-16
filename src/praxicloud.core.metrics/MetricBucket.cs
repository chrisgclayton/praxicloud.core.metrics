// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    #region Using Clauses
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using praxicloud.core.math;
    #endregion

    /// <summary>
    /// A helper class to compute bucketed metric aggregates
    /// </summary>
    public sealed class MetricBucket
    {
        #region Variables
        /// <summary>
        /// The list of bucket values
        /// </summary>
        private readonly ConcurrentDictionary<long, List<double>> _bucketValues = new ConcurrentDictionary<long, List<double>>();
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="duration">The duration in seconds</param>
        public MetricBucket(long duration)
        {
            Duration = duration;
        }
        #endregion
        #region Properties
        /// <summary>
        /// The whole seconds of time in the duration
        /// </summary>
        public long Duration { get; }
        #endregion
        #region Methods
        /// <summary>
        /// Adds an observed value for the specified timestamp
        /// </summary>
        /// <param name="timestamp">The time that the value was observed at</param>
        /// <param name="value">The value that was observed</param>
        public void AddValue(DateTimeOffset timestamp, double value)
        {
            var bucket = GetBucket(timestamp, Duration);
            List<double> values = null;

            if(_bucketValues.TryGetValue(bucket, out values))
            {
                values.Add(value);
            }
            else
            {
                _bucketValues.TryAdd(bucket, new List<double>());
                _bucketValues.TryGetValue(bucket, out values);

                var removeBeforeBucket = bucket - 3;

                foreach(var pair in _bucketValues)
                {
                    if (pair.Key < removeBeforeBucket) _bucketValues.TryRemove(pair.Key, out _);
                }
            }

            if(values != null)
            {
                values.Add(value);
            }
        }

        /// <summary>
        /// Retrieves the metric aggregates for the latest complete bucket
        /// </summary>
        /// <param name="bucketStartTime">The start time of the bucket with completed aggregates (assumed by newer values arriving)</param>
        /// <param name="count">The number of metrics the aggregates are based on</param>
        /// <param name="maximum">The maximum value</param>
        /// <param name="minimum">The minimum value</param>
        /// <param name="mean">The average (mean) value</param>
        /// <param name="standardDeviation">The standard deviation of the values</param>
        /// <param name="p50">The 50th percentile of the values</param>
        /// <param name="p90">The 90th percentile of the values</param>
        /// <param name="p95">The 95th percentile of the values</param>
        /// <param name="p98">The 98th percentile of the values</param>
        /// <param name="p99">The 99th percentile of the values</param>
        public void GetAggregates(out DateTimeOffset? bucketStartTime,  out int? count, out double? maximum, out double? minimum, out double? mean, out double? standardDeviation, out double? p50, out double? p90, out double? p95, out double? p98, out double? p99)
        {
            var targetKey = -1L; // _bucketValues.Count > 0 ? _bucketValues.Max(item => item.Key) - 1 : -1;

            if (_bucketValues.Count > 0)
            {
                var maxValue = -1L;
                var previousValue = -1L;
                var keyEnumerator = _bucketValues.Keys.GetEnumerator();

                while(keyEnumerator.MoveNext())
                {
                    var currentKey = keyEnumerator.Current;

                    if(currentKey > maxValue)
                    {
                        previousValue = maxValue;
                        maxValue = currentKey;
                    }
                    else if(currentKey > previousValue)
                    {
                        previousValue = currentKey;
                    }
                }

                targetKey = previousValue;
            }

            if(targetKey >= 0 && _bucketValues.TryGetValue(targetKey, out var values))
            {
                bucketStartTime = GetBucketTime(targetKey, Duration);
                count = values.Count;

                var dataValues = new double[count.Value];                

                for(var index = 0; index < dataValues.Length; index++)
                {
                    dataValues[index] = values[index];
                }

                Aggregates.GetPerformanceAggregates(dataValues, out maximum, out minimum, out mean, out _, out standardDeviation, out p50, out p90, out p95, out p98, out p99);               
            }
            else
            {
                maximum = null;
                minimum = null;
                mean = null;
                standardDeviation = null;
                p50 = null;
                p90 = null;
                p95 = null;
                p98 = null;
                p99 = null;
                count = null;
                bucketStartTime = null;
            }
        }

        /// <summary>
        /// Gets the bucket based on duration
        /// </summary>
        /// <param name="timeStamp">The timestamp to find the bucket for</param>
        /// <param name="duration">The duration in each bucket</param>
        /// <returns>The bucket index</returns>
        private static long GetBucket(DateTimeOffset timeStamp, long duration)
        {
            return (long)Math.Round((double)timeStamp.ToUnixTimeSeconds() / duration, MidpointRounding.ToZero);
        }

        /// <summary>
        /// Converts the calculated bucket number to the Date Time Offset
        /// </summary>
        /// <param name="bucket">The bucket</param>
        /// <param name="duration">The duration used to calculate the buckets</param>
        /// <returns></returns>
        private static DateTimeOffset GetBucketTime(long bucket, long duration)
        {
            return DateTimeOffset.FromUnixTimeSeconds(bucket * duration);
        }
        #endregion
    }
}
