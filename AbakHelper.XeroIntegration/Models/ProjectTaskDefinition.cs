using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AbakHelper.XeroIntegration.Models
{
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
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}