// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.metrics.tests
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics.CodeAnalysis;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TextUtilityTests
    {
        [DataTestMethod]
        [DataRow(true, true)]
        [DataRow(false, true, "label1")]
        [DataRow(false, true, "label1", "label2")]
        [DataRow(false, true, "label1", "label2", "label3")]
        [DataRow(false, false, "label1", "label2", "label3")]
        public void LabelText(bool labelsNull, bool useLabels, params string[] labels)
        {
            string[] testLabels = labelsNull ? null : labels;

            var text = TextOutputUtilities.GetLabelText(useLabels, testLabels);
            
            string expectedText = useLabels ? (testLabels?.Length ?? 0) == 0 ? TextOutputUtilities.EmptyLabelText : string.Join(TextOutputUtilities.LabelSeperator, testLabels) : "";

            Assert.IsNotNull(text, "Text not expected to be null");
            Assert.IsTrue(string.Equals(text, expectedText), "Text not expected");
        }

        [DataTestMethod]
        [DataRow(true, 0.0)]
        [DataRow(false, 0.0)]
        [DataRow(false, 1.0)]
        [DataRow(false, -1.0)]
        [DataRow(false, double.MinValue)]
        [DataRow(false, double.MaxValue)]
        [DataRow(false, -1234123.12321523123)]
        [DataRow(false, 1234123.12321523123)]
        public void DoubleValueText(bool valueNull, double value)
        {
            var text = TextOutputUtilities.GetValueText(valueNull ? (double?)null : value);
            string expectedText = valueNull ? TextOutputUtilities.NotNumber : value.ToString("0.0000");

            Assert.IsNotNull(text, "Text not expected to be null");
            Assert.IsTrue(string.Equals(text, expectedText), "Text not expected");
        }

        [DataTestMethod]
        [DataRow(true, 0)]
        [DataRow(false, 0)]
        [DataRow(false, 1)]
        [DataRow(false, -1)]
        [DataRow(false, int.MinValue)]
        [DataRow(false, int.MaxValue)]
        [DataRow(false, -1234123)]
        [DataRow(false, 1234123)]
        public void IntegerValueText(bool valueNull, int value)
        {
            var text = TextOutputUtilities.GetValueText(valueNull ? (int?)null : value);
            string expectedText = valueNull ? TextOutputUtilities.NotNumber : value.ToString("0");

            Assert.IsNotNull(text, "Text not expected to be null");
            Assert.IsTrue(string.Equals(text, expectedText), "Text not expected");
        }
    }
}
