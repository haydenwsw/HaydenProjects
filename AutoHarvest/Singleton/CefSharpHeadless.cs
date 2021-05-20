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
    public class CefSharpHeadless
    {
        /// <summary>
        /// The request context
        /// </summary>
        private RequestContext RequestContext { get; set; }

        // chromium does not manage timeouts, so we'll implement one
        private readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        // the page that the links are loaded on
        private readonly ChromiumWebBrowser Page;

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

            // create a new page and wait for Initializion
            Page = new ChromiumWebBrowser("", null, RequestContext);
            SpinWait.SpinUntil(() => Page.IsBrowserInitialized);
        }

        /// <summary>
        /// Open the given url
        /// </summary>
        /// <param name="url">the url</param>
        /// <returns></returns>
        public Task<string> GetHtmlAsync(string url)
        {
            try
            {
                Page.LoadingStateChanged += PageLoadingStateChanged;
                if (Page.IsBrowserInitialized)
                {
                    Page.Load(url);

                    // create a 10 sec timeout 
                    bool isSignalled = manualResetEvent.WaitOne(TimeSpan.FromSeconds(10));
                    manualResetEvent.Reset();

                    // As the request may actually get an answer, we'll force stop when the timeout is passed
                    if (!isSignalled)
                    {
                        Page.Stop();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // happens on the manualResetEvent.Reset(); when a cancelation token has disposed the context
            }
            Page.LoadingStateChanged -= PageLoadingStateChanged;

            // get the html
            return Page.GetBrowser().GetFrame("").GetSourceAsync();
        }

        /// <summary>
        /// Manage the IsLoading parameter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            // Check to see if loading is complete - this event is called twice, one when loading starts
            // second time when it's finished
            if (!e.IsLoading)
            {
                manualResetEvent.Set();
            }

        }
    }
}
