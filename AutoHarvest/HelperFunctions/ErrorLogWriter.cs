using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHarvest.HelperFunctions
{
    public static class ErrorLogWriter
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

        // use this when a scraper breaks (again)
        public static void WriteHtmlNodes(HtmlNode[] Htmlnodes)
        {
            StreamWriter scriptss = new StreamWriter("Nodes.txt", false);

            for (int i = 0; i < Htmlnodes.Length; i++)
            {
                scriptss.WriteLine($"[{i}]");
                scriptss.WriteLine(Htmlnodes[i].InnerText);
                scriptss.WriteLine("");
            }

            scriptss.Flush();
        }

        public static void WriteHtmlNodes(IEnumerable<HtmlNode> Htmlnodes)
        {
            StreamWriter scriptss = new StreamWriter("Nodes.txt", false);

            int i = 0;
            foreach (HtmlNode Htmlnode in Htmlnodes)
            {
                scriptss.WriteLine($"[{i++}]");
                scriptss.WriteLine(Htmlnode.InnerText);
                scriptss.WriteLine("");
            }

            scriptss.Flush();
        }
    }
}
