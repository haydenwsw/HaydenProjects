using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Singleton
{
    /// <summary>
    /// class for changing the title at certain dates
    /// </summary>
    public class Events
    {
        // the total amount of the times the privacy page has been visited
        public long TimesVisited = 0;

        // gets the title based on the event on that day
        public string GetTitle()
        {
            // get today's date
            string today = DateTime.Today.ToString("dd-MM");

            // look it up
            switch (today)
            {
                case "01-01":
                    return $"Happy New Year {DateTime.Today.ToString("yyyy")}";

                case "14-02":
                    return "Valentine's Day";

                case "14-03":
                    return "𝛑 Day";

                case "17-04":
                    return "Happy Easter";

                case "20-04":
                    return "Blaze it";

                case "09-05":
                    return "Mother's Day";

                case "13-05":
                    return "Happy Birthday Gibbs";

                case "01-06":
                    return "Happy Birthday Alex";

                case "20-06":
                    return "Father's Day";

                case "10-09":
                    return "Happy Birthday NiB";

                case "31-10":
                    return "Happy Halloween";

                case "21-12":
                    return "Happy Birthday to the creator";

                case "24-12":
                    return "Christmas Eve!";

                case "25-12":
                    return "Marry Christmas";

                case "31-12":
                    return "New Years Eve!";

                default:
                    return "Car Finder";
            }
        }
    }
}
