using System.Windows.Controls;

namespace AbakHelper.UserControls
{
    /// <summary>
    /// Interaction logic for InvoiceNumberDialog.xaml
    /// </summary>
    public partial class InvoiceNumberDialog : UserControl
    {
        public InvoiceNumberDialog(string mostRecentInvoiceNumber)
        {
            InitializeComponent();
            MostRecentTextBlock.Text += mostRecentInvoiceNumber;
        }
    }
}
