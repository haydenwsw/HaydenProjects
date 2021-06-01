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
        private readonly Dictionary<string, string> Dates = new Dictionary<string, string>
        {
            { "01-01", "Happy New Year" },

            { "14-02", "Valentine's Day" },

            { "07-03", "Happy Birthday Croucher" },

            { "17-04", "Happy Easter" },

            { "20-04", "Blaze it" },

            { "09-05", "Mother's Day" },

            { "13-05", "Happy Birthday Gibbs" },

            { "01-06", "Happy Birthday Alex" },

            { "20-06", "Father's Day" },

            { "10-09", "Happy Birthday NiB" },

            { "10-31", "Happy Halloween" },

            { "21-12", "Happy Birthday to the creator" },

            { "24-12", "Christmas Eve!" },

            { "25-12", "Marry Christmas" },

            { "31-12", "New Years Eve!" }
        };

        // gets the title based on the event on that day
        public string GetTitle()
        {
            // get today's date
            string today = DateTime.Today.ToString("dd-MM");

            // look it up
            if (Dates.TryGetValue(today, out string title))
                return title;
            else
                return "Auto Harvest";
        }
    }
}
