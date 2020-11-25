using System.Collections.Generic;
using AbakHelper.Integration.Models;
using AbakHelper.XeroIntegration.Models;

namespace AbakHelper.XeroIntegration.ViewModels
{
    public class ProjectExportViewModel
    {
        public Project Project { get; set; }

        public List<TaskDefinitionExportViewModel> TaskDefinitions { get; set; }
    }
}