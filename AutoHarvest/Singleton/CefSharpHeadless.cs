using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using CefSharp;
using CefSharp.OffScreen;

namespace AutoHarvest.Singleton
{
    /// <summary>
    /// Headless browser used the load html from ticky websites TODO: tab pooling
    /// </summary>
    public class CefSharpHeadless
    {
        // The request context
        private RequestContext RequestContext { get; set; }

        public CefSharpHeadless()
        {
            var settings = new CefSettings()
            {
                // By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
            };

            // Autoshutdown when closing
            CefSharpSettings.ShutdownOnExit = true;

            // Perform dependency check to make sure all relevant resources are in our     output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            RequestContext = new RequestContext();
        }

        /// <summary>
        /// Creates an empty tab
        /// </summary>
        /// <returns></returns>
        public ChromiumWebBrowser CreateNewTab()
        {
            ChromiumWebBrowser tab = new ChromiumWebBrowser("", null, RequestContext);
            SpinWait.SpinUntil(() => tab.IsBrowserInitialized);

            return tab;
        }

        /// <summary>
        /// Gets the html from a url
        /// </summary>
        /// <param name="url">the url</param>
        /// <returns>the html</returns>
        public static Task<string> GetHtmlAsybc(ChromiumWebBrowser Tab, string url)
        {
            // chromium does not manage timeouts, so we'll implement one
            ManualResetEvent manualResetEventt = new ManualResetEvent(false);

            try
            {
                Tab.LoadingStateChanged += PageLoadingStateChanged;
                if (Tab.IsBrowserInitialized)
                {
                    Tab.Load(url);

                    // create a 10 sec timeout 
                    bool isSignalled = manualResetEventt.WaitOne(TimeSpan.FromSeconds(8));
                    manualResetEventt.Reset();

                    // As the request may actually get an answer, we'll force stop when the timeout is passed
                    if (!isSignalled)
                    {
                        Tab.Stop();
                        Tab.LoadingStateChanged -= PageLoadingStateChanged;
                        throw new TimeoutException("The request has exceeded the 8 second timeout rule.");
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // happens on the manualResetEvent.Reset(); when a cancelation token has disposed the context
            }

            Tab.LoadingStateChanged -= PageLoadingStateChanged;

            // get the html
            return Tab.GetBrowser().GetFrame("").GetSourceAsync();

            // Manage the IsLoading parameter
            void PageLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
            {
                // Check to see if loading is complete - this event is called twice, one when loading starts
                // second time when it's finished
                if (!e.IsLoading)
                {
                    manualResetEventt.Set();
                }

            }
        }
    }
}
