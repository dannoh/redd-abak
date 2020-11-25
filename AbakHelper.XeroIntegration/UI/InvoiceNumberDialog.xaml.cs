using System.Windows.Controls;

namespace AbakHelperV2.UserControls
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
