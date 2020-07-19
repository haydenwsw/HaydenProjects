using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.HelperFunctions
{
    public class ErrorLogWriter
    {
        // write to the log files
        internal static StreamWriter ErrorLog = new StreamWriter("ErrorLog.txt", true);

        public static void WriteLog(Exception exc)
        {
            // log the exception
            ErrorLog.WriteLine(exc.Message);
            ErrorLog.WriteLine(exc.StackTrace);
            ErrorLog.WriteLine(exc.Source);
            ErrorLog.WriteLine("");
            ErrorLog.Flush();
        }
    }
}
