using System.Windows.Controls;

namespace AbakHelper.JsonExport
{
    /// <summary>
    /// Interaction logic for JsonViewer.xaml
    /// </summary>
    public partial class JsonViewer : UserControl
    {
        public JsonViewer(string json)
        {
            InitializeComponent();
            textBox.Text = json;
        }
    }
}
