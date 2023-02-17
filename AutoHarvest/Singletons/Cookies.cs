using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.Singletons
{
    /// <summary>
    /// Holds all the web cookies for webscraping
    /// Don't upload your cookies to the internet
    /// </summary>
    public class Cookies
    {
        public readonly string FbMarketplace;

        private const string cookiePath = "./cookies.txt";

        public Cookies()
        {
            File.AppendAllText(cookiePath, "");

            string[] cookies = File.ReadAllLines(cookiePath);

            if (cookies.Length > 0)
            {
                FbMarketplace = cookies[0];
            }
        }


    }
}
