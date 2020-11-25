using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbakHelperV2.Annotations;
using AbakHelperV2.Infrastructure;

namespace AbakHelperV2.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event EventHandler Save = (s, e) => { };

        private string _abakUrl;
        public string AbakUrl
        {
            get => _abakUrl;
            set
            {
                if (value == _abakUrl) return;
                _abakUrl = value;
                OnPropertyChanged();
            }
        }

        public Command SaveCommand { get; }
        public ObservableCollection<ComponentSettingsViewModel> ComponentSettings { get; }

        public SettingsViewModel(IEnumerable<ComponentSettingsViewModel> componentSettings)
        {
            //TODO:DF Even if you don't save, we are still updating the objects... should clone or something...
            ComponentSettings = new ObservableCollection<ComponentSettingsViewModel>(componentSettings);
            SaveCommand = new Command(ExecuteSaveCommand);
        }


        private void ExecuteSaveCommand(object obj)
        {
            Save(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
