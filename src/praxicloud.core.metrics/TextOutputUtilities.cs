// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics
{
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// Helper utilities for text based writerse
    /// </summary>
    public static class TextOutputUtilities
    {
        #region Constants
        /// <summary>
        /// The text used to represent an empty or null label
        /// </summary>
        public const string EmptyLabelText = "[]";

        /// <summary>
        /// The text used to represent a label that is not included
        /// </summary>
        public const string NotIncludedLabelText = "";

        /// <summary>
        /// The separator used between the labels
        /// </summary>
        public const string LabelSeperator = @":";

        /// <summary>
        /// Text that represents a scenario where the value is not a number
        /// </summary>
        public const string NotNumber = @"NaN";

        /// <summary>
        /// The format string used to format double values
        /// </summary>
        public const string DoubleFormat = @"0.0000";

        /// <summary>
        /// The format string used to format integer values
        /// </summary>
        public const string IntegerFormat = @"0";
        #endregion
        #region Methods
        /// <summary>
        /// Builds the text to insert as the label text
        /// </summary>
        /// <param name="labels">The labels object</param>
        /// <param name="includeLabels">True if the labels text should be included for the counter</param>
        /// <returns>The text to insert</returns>
        public static string GetLabelText(bool includeLabels, string[] labels)
        {
            string labelText;

            if (includeLabels)
            {
                if ((labels?.Length ?? 0) > 0)
                {
                    var builder = new StringBuilder();

                    foreach (var label in labels)
                    {
                        if (builder.Length > 0) builder.Append(LabelSeperator);
                        builder.Append(label);
                    }

                    labelText = builder.ToString();
                }
                else
                {
                    labelText = EmptyLabelText;
                }
            }
            else
            {
                labelText = NotIncludedLabelText;
            }

            return labelText;
        }

        /// <summary>
        /// Converts a double to a string in the appropriate format or replaces with not a number representation
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The string representation</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetValueText(double? value)
        {
            return value.HasValue? value.Value.ToString(DoubleFormat) : NotNumber;
        }

        /// <summary>
        /// Converts an integer to a string in the appropriate format or replaces with not a number representation
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The string representation</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetValueText(int? value)
        {
            return value.HasValue ? value.Value.ToString(IntegerFormat) : NotNumber;
        }
        #endregion
    }
}
