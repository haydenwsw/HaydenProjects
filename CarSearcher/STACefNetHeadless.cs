using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CarSearcher.Models;
using CefNet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CarSearcher
{
    internal class CefApplication : CefNetApplication
    {
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            base.OnBeforeCommandLineProcessing(processType, commandLine);

            // disbale js for speed and efficiency
            commandLine.AppendSwitch("disable-javascript");

            commandLine.AppendSwitch("disable-remote-fonts");
            commandLine.AppendSwitch("disable-image-loading");

            // asp .net shenanigans
            commandLine.AppendSwitch("single-process");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                commandLine.AppendSwitch("no-zygote");
                commandLine.AppendSwitch("no-sandbox");
            }
        }
    }

    /// <summary>
    /// Headless Chromium browser used to scrape html from tricky websites. 
    /// </summary>
    public class STACefNetHeadless
    {
        private class Page
        {
            public string Url { get; set; }
            public string Html { get; set; }
        }

        private ManualResetEvent ManualResetEvent;
        private long Ids = 0;
        // thread safe dict?
        private Dictionary<long, Page> PageDict;

        /// <summary>
        /// Initialize Chromium
        /// </summary>
        public STACefNetHeadless(IOptions<CarSearcherConfig> carsearcherconfig, ILogger<STACefNetHeadless> logger)
        {
            // get project path
            string startupPath = Environment.CurrentDirectory;
            string cefPath = Path.Combine(startupPath, carsearcherconfig.Value.Folder, "cef");

            // configure setting
            CefSettings settings = new CefSettings
            {
                MultiThreadedMessageLoop = false,
                NoSandbox = true,
                WindowlessRenderingEnabled = true,
                LocalesDirPath = Path.Combine(cefPath, "Resources", "locales"),
                ResourcesDirPath = Path.Combine(cefPath, "Resources")
            };

            // Copy cef/Resources/icudtl.dat into cef/Release/
            CefApplication app = new CefApplication();

            ManualResetEvent = new ManualResetEvent(false);
            bool isDone = false;

            Ids = 0;
            PageDict = new Dictionary<long, Page>(); ;

            Thread thread = new Thread(() =>
            {
                app.Initialize(Path.Combine(cefPath, "Release"), settings);
                isDone = ManualResetEvent.Reset();
                ManualResetEvent.WaitOne();

                while (true)
                {
                    try
                    {
                        Page page = PageDict.FirstOrDefault().Value;

                        WindowlessWebView tab = new WindowlessWebView();
                        SpinWait.SpinUntil(() => { CefApi.DoMessageLoopWork(); return !tab.IsBusy; });

                        tab.Navigate(page.Url);

                        SpinWait.SpinUntil(() => { CefApi.DoMessageLoopWork(); return !tab.BrowserObject.IsLoading; });

                        Task<string> task = tab.GetMainFrame().GetSourceAsync(CancellationToken.None);
                        SpinWait.SpinUntil(() => { CefApi.DoMessageLoopWork(); return task.IsCompleted; });

                        page.Html = task.Result;
                        tab.Close();

                        if (PageDict.FirstOrDefault().Value == null)
                        {
                            PageDict.TrimExcess();
                            ManualResetEvent.Reset();
                            ManualResetEvent.WaitOne();
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError("CefNetHeadless has failed to get the html", e);
                        // wipe so do inf loops
                        PageDict.Clear();
                    }
                }
            });
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            SpinWait.SpinUntil(() => isDone);
        }

        /// <summary>
        /// Creates a new tab loads the url then gets the html
        /// </summary>
        /// <param name="url">Website uri</param>
        /// <returns></returns>
        public string NewTabGetHtml(string url)
        {
            long id = GenerateID();

            PageDict.Add(id, new Page { Url = url });

            ManualResetEvent.Set();
            SpinWait.SpinUntil(() => PageDict[id].Html != null);

            string html = PageDict[id].Html;
            PageDict.Remove(id);

            return html;
        }

        /// <summary>
        /// Super advanced algorithem for generating ids
        /// </summary>
        /// <returns></returns>
        private long GenerateID()
        {
            return Ids++;
        }
    }
}
