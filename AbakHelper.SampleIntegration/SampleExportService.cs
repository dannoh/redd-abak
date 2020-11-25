using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using AbakHelper.Integration;
using AbakHelper.Integration.Models;

namespace AbakHelper.SampleIntegration
{
    public class SampleExportService : ExportServiceBase
    {
        public override string Name => "Sample Export";

        public override Task Export(List<TimeTrackerItem> tasks, ISettingsRepository settingsRepo)
        {
            SampleDisplayControl control = new SampleDisplayControl {DataContext = tasks};
            Window window = new Window {Content = control};
            window.Show();
            return Task.CompletedTask;
        }
    }
}
