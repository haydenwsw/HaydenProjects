using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.HelperFunctions
{
    public static class ExtentionFunctions
    {
        // all the legal charaters for string to uint function
        internal static readonly char[] legalChars = "1234567890".ToCharArray();

        /// <summary>
        /// returns all the numbers in the string as one number ignores all the other characters
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static uint toUInt(this string str)
        {
            uint number = 0;
            uint count = 1;
            bool dot = true;

            // iterate backwards through the string and parse the numbers
            for (int i = str.Length - 1; i > -1; i--)
            {
                // Todo: use hashing
                if (legalChars.Contains(str[i]))
                {
                    uint num = (uint)str[i] - 48;
                    number = num * count + number;
                    count *= 10;
                }
                // gets rid of the decimal numbers and just incase there are more then one '.'
                else if (str[i] == '.' && dot && count != 1)
                {
                    number = 0;
                    count = 1;
                    dot = false;
                }
            }

            return number;
        }
    }
}
