using System.Windows.Controls;

namespace AbakHelperV2.ViewModels
{
    public class ComponentSettingsViewModel
    {
        public string Name { get; }
        public UserControl Content { get; }

        public ComponentSettingsViewModel(string name, UserControl content)
        {
            Name = name;
            Content = content;
        }
    }
}