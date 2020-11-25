using System.Windows;
using System.Windows.Controls;
using AbakHelper.Integration.Models;
using AbakHelper.XeroIntegration.ViewModels;
using Xero.Api.Core.Model;

namespace AbakHelper.XeroIntegration.UI
{
    /// <summary>
    /// Interaction logic for ExportView.xaml
    /// </summary>
    public partial class ExportView : UserControl
    {
        public ExportView()
        {
            InitializeComponent();
            
        }

        public void SetDataContext(ExportViewModel viewModel)
        {
            DataContext = viewModel;
            //TODO:DF Port
            viewModel.ShowInvoice += OnInvoiceCreated;
        }

        private void OnInvoiceCreated(object sender, EventArgs<Invoice> e)
        {
            if (e.Data == null)
            {
                webBrowser.Navigate("about:blank");
            }
            else
            {
                webBrowserContainer.Width = ActualWidth / 2;
                webBrowserContainer.Visibility = Visibility.Visible;
                var path = $"file:///{System.AppDomain.CurrentDomain.BaseDirectory}{e.Data.Number}.pdf";
                webBrowser.Tag = e.Data;
                webBrowser.Navigate(path);
            }
        }

    }
}
