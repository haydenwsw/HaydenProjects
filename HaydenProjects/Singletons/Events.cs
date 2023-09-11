using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HaydenProjects.Singletons
{
    /// <summary>
    /// Changes the title at certain dates
    /// </summary>
    public class Events
    {
        private int Year;
        private string EasterDate;
        private string MothersDayDate;
        private string FathersDayDate;
        private readonly Dictionary<string, string> TitleLookup;

        public Events()
        {
            Year = DateTime.Today.Year;
            EasterDate = CalculateEasterDate();
            MothersDayDate = CalculateMothersDay();
            FathersDayDate = CalculateFathersDay();

            TitleLookup = new Dictionary<string, string>()
            {
                { "01-01", $"Happy New Year {Year}" },
                { "26-01", "Australia Day" },
                { "14-02", "Valentine's Day" },
                { "14-03", "𝛑 Day" },
                { EasterDate, "Happy Easter" },
                { "20-04", "Blaze it" },
                { "25-04", "Anzac Day" },
                { MothersDayDate, "Mother's Day" },
                { "13-05", "Happy Birthday Gibbs" },
                { "01-06", "Happy Birthday Alex" },
                { "12-06", "Kings Birthday" },
                { FathersDayDate, "Father's Day" },
                { "10-09", "Happy Birthday NiB" },
                { "02-10", "Labor Day" },
                { "31-10", "Happy Halloween" },
                { "21-12", "Happy Birthday to me" },
                { "24-12", "Christmas Eve!" },
                { "25-12", "Marry Christmas" },
                { "31-12", "New Years Eve!" }
            };
        }

        // gets the title based on the event on that day
        public string GetTitle(string title)
        {
            // get today's date
            string today = DateTime.Today.ToString("dd-MM");

            if (Year != DateTime.Today.Year)
                RecalculateDates();

            // look it up
            if (TitleLookup.TryGetValue(today, out string coolTitle))
                return coolTitle;

            return title;
        }

        private void RecalculateDates()
        {
            Year = DateTime.Today.Year;

            TitleLookup["01-01"] = $"Happy New Year {Year}";

            // recalc Easter
            TitleLookup.Remove(EasterDate);
            EasterDate = CalculateEasterDate();
            TitleLookup.Add(EasterDate, "Happy Easter");

            // recalc mothers day
            TitleLookup.Remove(MothersDayDate);
            MothersDayDate = CalculateMothersDay();
            TitleLookup.Add(MothersDayDate, "Mother's Day");

            // recalc fathers day
            TitleLookup.Remove(FathersDayDate);
            FathersDayDate = CalculateFathersDay();
            TitleLookup.Add(FathersDayDate, "Father's Day");
        }

        // calculates the Easter date
        private string CalculateEasterDate()
        {
            int easterNumber = Year - Year / 19 * 19 + 1;

            // Easter table
            DateTime easterDate;
            switch (easterNumber)
            {
                case 0:
                    easterDate = DateTime.Parse($"{Year}/03/27");
                    break;
                case 1:
                    easterDate = DateTime.Parse($"{Year}/04/14");
                    break;
                case 2:
                    easterDate = DateTime.Parse($"{Year}/04/03");
                    break;
                case 3:
                    easterDate = DateTime.Parse($"{Year}/03/23");
                    break;
                case 4:
                    easterDate = DateTime.Parse($"{Year}/04/11");
                    break;
                case 5:
                    easterDate = DateTime.Parse($"{Year}/03/31");
                    break;
                case 6:
                    easterDate = DateTime.Parse($"{Year}/04/18");
                    break;
                case 7:
                    easterDate = DateTime.Parse($"{Year}/04/08");
                    break;
                case 8:
                    easterDate = DateTime.Parse($"{Year}/03/28");
                    break;
                case 9:
                    easterDate = DateTime.Parse($"{Year}/04/16");
                    break;
                case 10:
                    easterDate = DateTime.Parse($"{Year}/04/05");
                    break;
                case 11:
                    easterDate = DateTime.Parse($"{Year}/03/25");
                    break;
                case 12:
                    easterDate = DateTime.Parse($"{Year}/04/13");
                    break;
                case 13:
                    easterDate = DateTime.Parse($"{Year}/04/02");
                    break;
                case 14:
                    easterDate = DateTime.Parse($"{Year}/03/22");
                    break;
                case 15:
                    easterDate = DateTime.Parse($"{Year}/04/10");
                    break;
                case 16:
                    easterDate = DateTime.Parse($"{Year}/03/30");
                    break;
                case 17:
                    easterDate = DateTime.Parse($"{Year}/04/17");
                    break;
                case 18:
                    easterDate = DateTime.Parse($"{Year}/04/07");
                    break;
                case 19:
                    easterDate = DateTime.Parse($"{Year}/03/27");
                    break;
                default:
                    easterDate = new DateTime();
                    break;
            }

            // calculate the upcoming Sunday
            int days = 7 - (int)easterDate.DayOfWeek;
            return easterDate.AddDays(days).ToString("dd-MM");
        }

        // calculates the mothers day date
        private string CalculateMothersDay()
        {
            DateTime MothersDay = DateTime.Parse($"{Year}/05/01");

            // calculate the second Sunday May
            int days = 14 - (int)MothersDay.DayOfWeek;
            return MothersDay.AddDays(days).ToString("dd-MM");
        }

        // calculates the fathers day date
        private string CalculateFathersDay()
        {
            DateTime FathersDay = DateTime.Parse($"{Year}/09/01");

            // calculate the first Sunday September
            int days = 7 - (int)FathersDay.DayOfWeek;
            return FathersDay.AddDays(days).ToString("dd-MM");
        }
    }
}
