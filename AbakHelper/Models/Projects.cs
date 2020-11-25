using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbakHelper.Annotations;

namespace AbakHelper.Models
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
        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProjectTaskDefinition : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                NotifyPropertyChanged();
            }
        }

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


        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProjectTask
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DisplayDesc { get; set; }
        public bool IsBillable { get; set; }
        public bool IsHourly { get; set; }
        public double Quantity { get; set; }
        public DateTime Date { get; set; }
        public Project Project { get; set; }
        public ProjectTaskDefinition TaskDefinition { get; set; }
    }
}
