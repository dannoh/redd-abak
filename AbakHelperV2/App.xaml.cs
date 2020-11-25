using System;
using System.IO;
using System.Reflection;
using System.Windows;
using AbakHelper.Integration;
using CefSharp;
using CefSharp.Wpf;

using NLog;

namespace AbakHelperV2
{
    public partial class App
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            //Example of setting a command line argument
            //Enables WebRTC
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");

            //Perform dependency check to make sure all relevant resources are in our output directory.
            if (!Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null))
                throw new Exception("Dan");
        }


        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception);
        }
    }
}
