using System;
using System.Collections.Generic;
using System.Text;
using AutoHarvest.HelperFunctions;
using Xunit;

namespace AutoHarvest.Tests
{
    public class TestExtentionFunctions
    {
        [Theory]
        [InlineData("10", 10)]
        [InlineData("$10 dollars", 10)]
        [InlineData(".1234", 0)]
        [InlineData("10abc12", 10)]
        [InlineData("10.10.10.10", 10)]
        [InlineData("...10...", 10)]
        [InlineData(null, -1)]
        [InlineData("", 0)]
        public void TestToInt(string input, int output)
        {
            int num = input.ToInt();

            Assert.Equal(output, num);
        }

        [Theory]
        [InlineData("10", 10)]
        [InlineData("10.1", 10.1f)]
        [InlineData("11.", 11)]
        [InlineData(".1234", .1234f)]
        [InlineData("abc10.4km", 10.4f)]
        [InlineData("10.1abc10.2", 10.1f)]
        [InlineData("...10...", 10)]
        [InlineData("10.10.14", 10.1f)]
        [InlineData(null, -1)]
        [InlineData("", -1)]
        public void TestToFloat(string input, float output)
        {
            float num = input.ToFloat();

            Assert.Equal(output, num);
        }

        [Theory]
        [InlineData("10", "10")]
        [InlineData("10.1", "101")]
        [InlineData("11.", "11")]
        [InlineData(".1234", "1234")]
        [InlineData("abc10.4km", "104")]
        [InlineData("10.1abc10.2", "101102")]
        [InlineData("...10...", "10")]
        [InlineData("10.10.14", "101014")]
        [InlineData(null, "")]
        [InlineData("", "")]
        public void TestLeaveOnlyNumbers(string input, string output)
        {
            string str = input.LeaveOnlyNumbers();

            Assert.Equal(output, str);
        }
    }
}
