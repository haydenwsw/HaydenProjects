using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web;
using System.Text;
using System.Globalization;

namespace AutoHarvest.HelperFunctions
{
    /// <summary>
    /// All extension functions (should change this to just string extentions)
    /// </summary>
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

        /// <summary>
        /// Converts a string that has been html encoded into a decoded string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string str)
        {
            return HttpUtility.HtmlDecode(str);
        }

        /// <summary>
        /// removes all the special charaicter like (é, á) and turn them into (a , e)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string str)
        {
            var normalizedString = str.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
