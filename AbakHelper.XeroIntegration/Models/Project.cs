using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AbakHelper.XeroIntegration.Models
{
    public class Project : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string ClientName { get; set; }

        private decimal _rate;
        public decimal Rate
        {
            get { return _rate; }
            set
            {
                if (value == _rate) return;
                _rate = value;
                NotifyPropertyChanged();
            }
        }

        public List<ProjectTaskDefinition> TaskDefinitions { get; set; }

        public Project()
        {
            TaskDefinitions = new List<ProjectTaskDefinition>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}