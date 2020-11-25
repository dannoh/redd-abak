using System.Collections.Generic;
using System.IO;
using AbakHelper.Integration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AbakHelperV2.Services
{

    public class SettingsRepository : ISettingsRepository
    {
        private readonly Settings _settings;
        private string _settingsFileName = "Settings.json";

        public SettingsRepository()
        {
            _settings = File.Exists(_settingsFileName) ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_settingsFileName)) : new Settings();
        }

        public void Save()
        {
            File.WriteAllText(_settingsFileName, JsonConvert.SerializeObject(_settings));
        }
        public void Save<T>(ExportServiceBase component, T settings)
        {
            _settings.ComponentSettings[component.Name] = settings;
            Save();
        }

        public Settings Get()
        {
            return _settings;
        }

        public T GetComponentSettings<T>(ExportServiceBase component)
        {
            var settings = (JObject)_settings.ComponentSettings[component.Name];
            if (settings == null)
                return default(T);
            return settings.ToObject<T>();
        }
    }
}
