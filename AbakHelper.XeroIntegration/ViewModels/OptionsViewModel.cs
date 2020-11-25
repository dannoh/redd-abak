using System.Collections.Generic;
using AbakHelper.XeroIntegration.Models;

namespace AbakHelper.XeroIntegration.ViewModels
{
    public class OptionsViewModel
    {
        
        public XeroSettings XeroSettings { get; }
        public IList<Project> Projects { get; }

        public OptionsViewModel(XeroSettings settings, IList<Project> projects)
        {
            Projects = projects;
            XeroSettings = settings;
        }
        
    }
}
