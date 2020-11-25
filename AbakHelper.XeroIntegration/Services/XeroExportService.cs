using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AbakHelper.Integration;
using AbakHelper.Integration.Models;
using AbakHelper.XeroIntegration.Models;
using AbakHelper.XeroIntegration.UI;
using AbakHelper.XeroIntegration.ViewModels;
using AbakHelperV2.UserControls;

namespace AbakHelper.XeroIntegration.Services
{
    public class XeroExportService : ExportServiceBase
    {
        private readonly ProjectRepository _projectRepo;
        public override string Name => "Xero";
        public override bool HasSettingsComponent => true;

        public XeroExportService()
        {
            _projectRepo = new ProjectRepository();
        }

        public override Task Export(List<TimeTrackerItem> tasks, ISettingsRepository settingsRepo)
        {
            var settings = settingsRepo.GetComponentSettings<XeroSettings>(this);
            List<ProjectTask> tracked = new List<ProjectTask>();
            foreach (var record in tasks)
            {
                var projectName = record.ProjectName;
                var clientName = record.ClientName;
                var project = _projectRepo.Get(projectName);
                if (project == null)
                {
                    project = new Project { ClientName = clientName, Name = projectName, Rate = 0 };
                    _projectRepo.Add(project);
                    _projectRepo.Save();
                    //Prompt for rate with a popup;
                }
                var taskDefinition = project.TaskDefinitions.FirstOrDefault(c => c.Name == record.DisplayDesc);
                if (taskDefinition == null)
                {
                    taskDefinition = new ProjectTaskDefinition { Name = record.DisplayDesc, Rate = project.Rate };
                    project.TaskDefinitions.Add(taskDefinition);
                    _projectRepo.Save();
                }

                if (taskDefinition.Rate == 0)
                    taskDefinition.Rate = project.Rate; //TODO:DF Rate should be removed from this
                tracked.Add(new ProjectTask
                {
                    Description = record.DisplayDesc,
                    IsBillable = record.IsBillable,
                    IsHourly = record.IsHourly,
                    Name = record.DisplayDesc,
                    Quantity = record.Quantity,
                    Date = record.Date,
                    Project = project,
                    TaskDefinition = taskDefinition
                });
            }

            var exportView = new ExportView();
            exportView.SetDataContext(new ExportViewModel(tracked, _projectRepo, settings));
            Window window = new Window {Content = exportView};
            window.Show();
            return Task.CompletedTask;
        }


        public override void SettingSaveCompleted(ISettingsRepository settingsRepo)
        {
            base.SettingSaveCompleted(settingsRepo);
            _projectRepo.Save();
        }

        public override UserControl GetSettingsComponent(ISettingsRepository settingsRepo)
        {
            var settings = settingsRepo.GetComponentSettings<XeroSettings>(this) ?? new XeroSettings();
            var viewModel = new OptionsViewModel(settings, _projectRepo.All());
            return new OptionsView {DataContext = viewModel};
        }
    }
}
