using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using AbakHelper.Integration.Models;

namespace AbakHelper.Integration
{
    public abstract class ExportServiceBase
    {
        public abstract string Name { get; }
        public virtual string DisplayName => Name;
        public abstract Task Export(List<TimeTrackerItem> tasks, ISettingsRepository settingsRepo);
        public abstract bool HasSettingsComponent { get; }
        public virtual void SettingSaveCompleted(ISettingsRepository settingsRepo) { }
        public virtual UserControl GetSettingsComponent(ISettingsRepository settingsRepo)
        {
            return null;
        }


    }
}
