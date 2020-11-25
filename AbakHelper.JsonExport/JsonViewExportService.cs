using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using AbakHelper.Integration;
using AbakHelper.Integration.Models;
using Newtonsoft.Json;

namespace AbakHelper.JsonExport
{
    public class JsonViewExportService : ExportServiceBase
    {
        public override string Name => "View JSON Export";
        public override bool HasSettingsComponent => false;

        public override Task Export(List<TimeTrackerItem> tasks, ISettingsRepository settingsRepo)
        {
            var json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
            Window window = new Window {Content = new JsonViewer(json)};
            window.Show();
            return Task.CompletedTask;
        }
    }
}