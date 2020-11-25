using System.IO;
using Newtonsoft.Json;

namespace AbakHelper.Services
{
    public class SettingsRepository
    {
        private Settings _settings;
        private string _settingsFileName = "Settings.json";

        public SettingsRepository()
        {
            _settings = File.Exists(_settingsFileName) ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_settingsFileName)) : new Settings();
        }

        public Settings Get()
        {
            return _settings;
        }

        public void Save(Settings settings)
        {
            _settings = settings;
            File.WriteAllText(_settingsFileName, JsonConvert.SerializeObject(_settings));
        }

      
    }
}
