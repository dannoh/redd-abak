using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using AbakHelper.Integration;
using AbakHelper.Integration.Models;
using AbakHelperV2.Services;
using AbakHelperV2.ViewModels;
using Awesomium.Core;

namespace AbakHelperV2
{
    public partial class MainWindow
    {
        public ObservableCollection<ExportServiceBase> ExportTargets { get; }

        private List<TimeTrackerItem> _currentlyTrackedProjectTasks;
        private readonly SettingsRepository _settingsRepo;

        public MainWindow()
        {
            ExportTargets = new ObservableCollection<ExportServiceBase>();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            InitializeComponent();
            DataContext = this;

            _settingsRepo = new SettingsRepository();
            //TODO:DF Port?            Projects = new ObservableCollection<Project>(_projectRepo.All());
            WebCore.Initialized += WebCore_Initialized;
            LoadPlugins();
            InitializeComponent();
            var settings = _settingsRepo.Get();
            if (!string.IsNullOrWhiteSpace(settings.AbakUrl))
            {
                webBrowser.Source = new Uri(settings.AbakUrl);
            }
            else
            {
                OptionsButton_Click(null, null);
            }

        }

        private void LoadPlugins()
        {
            foreach (var dir in Directory.GetDirectories("Plugins"))
            {
                foreach (var file in Directory.GetFiles(dir, "AbakHelper*.dll"))
                {

                    var dlls = Assembly.LoadFile($"{AppDomain.CurrentDomain.BaseDirectory}{file}");

                    foreach (Type type in dlls.GetTypes())
                    {
                        if (typeof(ExportServiceBase).IsAssignableFrom(type))
                        {
                            var c = Activator.CreateInstance(type) as ExportServiceBase;
                            ExportTargets.Add(c);
                        }
                    }
                }
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Ignore missing resources
            if (args.Name.Contains(".resources"))
                return null;

            // check for assemblies already loaded
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;

            // Try to load by filename - split out the filename of the full assembly name
            // and append the base path of the original assembly (ie. look in the same dir)
            string filename = args.Name.Split(',')[0] + ".dll".ToLower();

            string asmFile = FindFileInPath(filename, ".\\Plugins");
            if (!string.IsNullOrEmpty(asmFile))
            {
                try
                {
                    return Assembly.LoadFrom(asmFile);
                }
                catch
                {
                    return null;
                }
            }

            // FAIL - not found
            return null;
        }
        private string FindFileInPath(string filename, string path)
        {
            filename = filename.ToLower();

            foreach (var fullFile in Directory.GetFiles(path))
            {
                var file = Path.GetFileName(fullFile).ToLower();
                if (file == filename)
                    return fullFile;

            }
            foreach (var dir in Directory.GetDirectories(path))
            {
                var file = FindFileInPath(filename, dir);
                if (!string.IsNullOrEmpty(file))
                    return file;
            }

            return null;
        }

        private void WebCore_Initialized(object sender, CoreStartEventArgs e)
        {
            var resourceInterceptor = new Interceptor(webBrowser);
            resourceInterceptor.TimeEntriesLoaded += OnTimeEntriesLoaded;
            WebCore.ResourceInterceptor = resourceInterceptor;
        }

        private void OnTimeEntriesLoaded(object sender, EventArgs<List<TimeTrackerItem>> e)
        {
            _currentlyTrackedProjectTasks = e.Data;
            MainSnackbar.MessageQueue.Enqueue($"Tracked {e.Data.Count} items.");
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            webBrowser.Visibility = Visibility.Collapsed;
            var settings = _settingsRepo.Get();
            var componentSettingsViewModels = ExportTargets
                .Where(c => c.HasSettingsComponent)
                .Select(c => new ComponentSettingsViewModel(c.DisplayName, c.GetSettingsComponent(_settingsRepo)));
            var viewModel = new SettingsViewModel(componentSettingsViewModels) { AbakUrl = settings.AbakUrl };
            optionsView.DataContext = viewModel;

            void OnSave(object s, EventArgs ev)
            {
                //TODO:DF leaky
                settings.AbakUrl = viewModel.AbakUrl;
                _settingsRepo.Save();
                viewModel.Save -= OnSave;
                foreach (var target in ExportTargets)
                {
                    target.SettingSaveCompleted(_settingsRepo);
                }

                if (webBrowser.Source == null)
                    webBrowser.Source = new Uri(settings.AbakUrl);
                webBrowser.Visibility = Visibility.Visible;
                optionsView.Visibility = Visibility.Collapsed;
            }

            viewModel.Save += OnSave;
            optionsView.Visibility = Visibility.Visible;
        }

        private void OnAbakButtonClick(object sender, RoutedEventArgs e)
        {
            webBrowser.Visibility = Visibility.Visible;
            //exportView.Visibility = Visibility.Collapsed;
            optionsView.Visibility = Visibility.Collapsed;
        }

        private void ExecuteExportButton_Click(object sender, RoutedEventArgs e)
        {
            var exportService = ((FrameworkElement)sender).DataContext as ExportServiceBase;
            exportService?.Export(_currentlyTrackedProjectTasks, _settingsRepo);
        }
    }
}
