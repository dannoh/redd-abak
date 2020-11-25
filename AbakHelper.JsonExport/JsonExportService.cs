using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AbakHelper.Integration;
using AbakHelper.Integration.Models;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace AbakHelper.JsonExport
{
    public class JsonExportService : ExportServiceBase
    {
        public override string Name => "JSON Export";
        public override bool HasSettingsComponent => false;
        

        public override Task Export(List<TimeTrackerItem> tasks, ISettingsRepository settingsRepo)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() ?? false)
            {
                var json = JsonConvert.SerializeObject(tasks);
                File.WriteAllText(dialog.FileName, json);
            }
            return Task.CompletedTask;
        }

    }
}
