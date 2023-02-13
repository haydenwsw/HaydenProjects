using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AutoHarvest.HelperFunctions
{
    public static class ExtentionFunctions
    {
        /// <summary>
        /// turns a string to an int dosen't support negitive numbers
        /// removes all characters after '.' and remove all non numbers
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            if (str == null)
                return -1;

            string nums = Regex.Replace(str, @"\..*$|[^\d]", "");

            if (nums == "")
                return 0;

            return int.Parse(nums);
        }

        /// <summary>
        /// turns a string to a float dosen't support negitive numbers
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static float ToFloat(this string str)
        {
            if (str == null)
                return -1;

            MatchCollection matches = Regex.Matches(str, @"^([^\.]*\.[^\.]*)");

            string nums;
            if (matches.Count == 0)
                nums = Regex.Replace(str, @"[^\d\.]", "");
            else
                nums = Regex.Replace(matches.First().Value, @"[^\d\.]", "");

            if (nums == "" || nums == ".")
                return -1;

            return float.Parse(nums);
        }

        /// <summary>
        /// removes all non numbers and leaves only numbers
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
