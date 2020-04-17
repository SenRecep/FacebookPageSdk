using CefSharp;
using CefSharp.Wpf;
using System;
using System.IO;
using System.Windows;

namespace FacebookPageSdk.WpfUi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var settings = new CefSettings()
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
                Locale="tr"
            };

            settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            settings.CefCommandLineArgs.Add("--js-flags", "--max_old_space_size=14000");

            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        }
    }
}
