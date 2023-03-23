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
        /// <param name="carsearcherconfig"></param>
        /// <param name="logger"></param>
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
                ResourcesDirPath = Path.Combine(cefPath, "Resources"),
                LogSeverity = CefLogSeverity.Error
            };

            // Copy cef/Resources/icudtl.dat into cef/Release/
            CefApplication app = new CefApplication();

            ManualResetEvent = new ManualResetEvent(false);
            bool isDone = false;

            Ids = 0;
            PageDict = new Dictionary<long, Page>(); ;

            // setup thread
            Thread thread = new Thread(() => CefNetMain(app, cefPath, settings, ref isDone, logger));

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
        /// CefNet's main thread
        /// i don't know why i don't want to know why
        /// but this is only way i could get it to work
        /// </summary>
        /// <param name="app"></param>
        /// <param name="cefPath"></param>
        /// <param name="settings"></param>
        /// <param name="isDone"></param>
        private void CefNetMain(CefApplication app, string cefPath, CefSettings settings, ref bool isDone, ILogger<STACefNetHeadless> logger)
        {
            try
            {
                app.Initialize(Path.Combine(cefPath, "Release"), settings);
                isDone = ManualResetEvent.Reset();
                ManualResetEvent.WaitOne();

                while (true)
                {
                    try
                    {
                        if (PageDict.FirstOrDefault().Value == null)
                        {
                            PageDict.TrimExcess();
                            ManualResetEvent.Reset();
                            ManualResetEvent.WaitOne();
                        }

                        Page page = PageDict.FirstOrDefault().Value;

                        WindowlessWebView tab = new WindowlessWebView();
                        SpinWait.SpinUntil(() => { CefApi.DoMessageLoopWork(); return !tab.IsBusy; });

                        tab.Navigate(page.Url);

                        SpinWait.SpinUntil(() => { CefApi.DoMessageLoopWork(); return !tab.BrowserObject.IsLoading; });

                        Task<string> task = tab.GetMainFrame().GetSourceAsync(CancellationToken.None);
                        SpinWait.SpinUntil(() => { CefApi.DoMessageLoopWork(); return task.IsCompleted; });

                        page.Html = task.Result;
                        tab.Close();
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "CefNetHeadless has failed to get the html");
                        // wipe so it doesn't inf loops
                        PageDict.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "CefNetHeadless has failed to Initialize");
                isDone = true;
            }
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
