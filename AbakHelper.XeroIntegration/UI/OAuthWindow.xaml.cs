using System.Text.RegularExpressions;
using System.Windows;
using CefSharp;

namespace AbakHelper.XeroIntegration.UI
{
    /// <summary>
    /// Interaction logic for OAuthWindow.xaml
    /// </summary>
    public partial class OAuthWindow
    {
        private readonly string _url;
        public string Pin { get; private set; }
        private bool _urlLoaded;
        public OAuthWindow(string url)
        {
            _url = url;
            InitializeComponent();
            Loaded += OAuthWindow_Loaded;
            webBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
        }

        private void WebBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (webBrowser.IsInitialized && !_urlLoaded)
            {
                //TODO:DF somehow we get here and it still throws saying its not initialized...
                _urlLoaded = true;
                webBrowser.Load(_url);
            }
        }

        private void OAuthWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (webBrowser.IsInitialized && !_urlLoaded)
            {
                //TODO:DF somehow we get here and it still throws saying its not initialized...
                _urlLoaded = true;
                webBrowser.Load(_url);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var source = await webBrowser.GetBrowser().MainFrame.GetSourceAsync();
            
            Match match = Regex.Match(source, "\\<input\\ id=\"pin\\-input\"\\ value=\"(?<pin>\\d+)\"\\ readonly=\"readonly\">");
            if (match.Success)
                Pin = match.Groups["pin"].Value;
            DialogResult = true;
            Close();
        }
    }
}
