﻿using System.Windows.Controls;

namespace AbakHelper.UserControls
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : UserControl
    {
        public MessageDialog(string message)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
        }
    }
}
