using System.Collections.Generic;
using System.Collections.ObjectModel;
using AbakHelper.Infrastructure;
using AbakHelper.Models;
using AbakHelper.Services;
using Newtonsoft.Json;

namespace AbakHelper.ViewModels
{
    public class OptionsViewModel : ViewModelBase
    {
        private readonly SettingsRepository _settingsRepo;
        private readonly ProjectRepository _projectRep;
        public Command SaveCommand { get; } 
        public Settings Settings { get; }

        public List<Project> Projects { get; }
        

        public OptionsViewModel(SettingsRepository settingsRepo, ProjectRepository projectRep)
        {
            _settingsRepo = settingsRepo;
            _projectRep = projectRep;
            Projects = JsonConvert.DeserializeObject<List<Project>>(JsonConvert.SerializeObject(_projectRep.All()));
            Settings = JsonConvert.DeserializeObject<Settings>(JsonConvert.SerializeObject(settingsRepo.Get()));
           
            SaveCommand = new Command(ExecuteSaveCommand);
        }

        private void ExecuteSaveCommand(object obj)
        {            
            _settingsRepo.Save(Settings);
            _projectRep.Save(Projects);
        }
    }
}
