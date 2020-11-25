using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AbakHelper.XeroIntegration
{
    public class XeroSettings : INotifyPropertyChanged
    {
        private string _companyName;
        public string CompanyName
        {
            get { return _companyName; }
            set
            {
                if (value == _companyName) return;
                _companyName = value;
                NotifyPropertyChanged();
            }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                if (value == _address) return;
                _address = value;
                NotifyPropertyChanged();
            }
        }

        private string _contactName;
        public string ContactName
        {
            get { return _contactName; }
            set
            {
                if (value == _contactName) return;
                _contactName = value;
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