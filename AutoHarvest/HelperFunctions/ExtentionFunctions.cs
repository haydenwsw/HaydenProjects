using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AutoHarvest.HelperFunctions
{
    public static class ExtentionFunctions
    {
        // all the legal charaters for string to uint function
        private static readonly HashSet<char> legalChars = "1234567890".ToHashSet();

        /// <summary>
        /// returns all the numbers in the string as one number ignores all the other characters
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            int number = 0;
            int count = 1;
            bool dot = true;

            // iterate backwards through the string and parse the numbers
            for (int i = str.Length - 1; i > -1; i--)
            {
                // using a hash set for instant lookup times
                if (legalChars.Contains(str[i]))
                {
                    int num = (int)str[i] - 48;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt2(this string str)
        {
            if (str == null)
                return -1;

            string nums = Regex.Replace(str, @"[^\d]", "");

            if (nums == "")
                return -1;

            return int.Parse(nums);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string LeaveOnlyNumbers(this string str)
        {
            if (str == null)
                return "";

            return Regex.Replace(str, @"[^\d]", "");
        }
    }
}
