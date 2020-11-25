using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AbakHelper.Integration.Models;
using AbakHelper.XeroIntegration.Models;

namespace AbakHelper.XeroIntegration.ViewModels
{
    public class TaskDefinitionExportViewModel : INotifyPropertyChanged
    {
        public ProjectTaskDefinition TaskDefinition { get; }
        public decimal Quantity {  get { return (decimal)Tasks.Sum(c => c.Quantity); } }
        public decimal Amount {  get { return Quantity * TaskDefinition.Rate; } }
        
        public List<ProjectTask> Tasks { get; set; }

        public TaskDefinitionExportViewModel(ProjectTaskDefinition definition)
        {
            TaskDefinition = definition;
            TaskDefinition.PropertyChanged += OnTaskDefinitionPropertyChanged;
        }

        private void OnTaskDefinitionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TaskDefinition.Rate))
                NotifyPropertyChanged(nameof(Amount));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}