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
        [InlineData(".1234", 0)]
        [InlineData("10.10.10.10", 10)]
        [InlineData("...10...", 10)]
        public void TestToUInt(string input, uint output)
        {
            uint num = input.toUInt();

            Assert.Equal(output, num);
        }
    }
}
