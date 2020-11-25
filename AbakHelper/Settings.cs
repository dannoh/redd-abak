using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbakHelper.Annotations;

namespace AbakHelper
{
    public class Settings : INotifyPropertyChanged
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

        private XeroSettings _xeroSettings;
        public XeroSettings XeroSettings
        {
            get { return _xeroSettings; }
            set
            {
                if (Equals(value, _xeroSettings)) return;
                _xeroSettings = value;
                NotifyPropertyChanged();
            }
        }

        public Settings()
        {
            XeroSettings = new XeroSettings();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class XeroSettings : INotifyPropertyChanged
    {
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

        private string _consumerSecret;
        public string ConsumerSecret
        {
            get { return _consumerSecret; }
            set
            {
                if (value == _consumerSecret) return;
                _consumerSecret = value;
                NotifyPropertyChanged();
            }
        }

        private string _consumerKey;
        public string ConsumerKey
        {
            get { return _consumerKey; }
            set
            {
                if (value == _consumerKey) return;
                _consumerKey = value;
                NotifyPropertyChanged();
            }
        }

        private string _certificatePath;
        public string CertificatePath
        {
            get { return _certificatePath; }
            set
            {
                if (value == _certificatePath) return;
                _certificatePath = value;
                NotifyPropertyChanged();
            }
        }

        private string _certificatePassword;
        public string CertificatePassword
        {
            get { return _certificatePassword; }
            set
            {
                if (value == _certificatePassword) return;
                _certificatePassword = value;
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
}