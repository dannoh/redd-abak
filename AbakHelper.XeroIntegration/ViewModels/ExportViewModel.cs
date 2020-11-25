using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AbakHelper.Integration;
using AbakHelper.Integration.Models;
using AbakHelper.Integration.UI;
using AbakHelper.XeroIntegration.Models;
using AbakHelper.XeroIntegration.Services;
using AbakHelperV2.Infrastructure;
using AbakHelperV2.UserControls;

using MaterialDesignThemes.Wpf;
using Xero.Api.Core.Model;

namespace AbakHelper.XeroIntegration.ViewModels
{
    public class ExportViewModel
    {
        public event EventHandler<EventArgs<Invoice>> ShowInvoice = (s, e) => { };
        
        private readonly ProjectRepository _projectRepo;
        private readonly XeroIntegrationService _integrationService;

        public ObservableCollection<ProjectExportViewModel> Projects { get; }

        public Command ExportToXeroCommand { get; }

        public List<IInvoiceCommand> Commands { get; }

        private object _currentInvoice;
        public object CurrentInvoice
        {
            get { return _currentInvoice; }
            private set
            {
                if (Equals(value, _currentInvoice)) return;
                _currentInvoice = value;
                NotifyPropertyChanged();
            }
        }

        public ExportViewModel(List<ProjectTask> tasks, ProjectRepository projectRepo, XeroSettings settings)
        {         
            _projectRepo = projectRepo;
            _integrationService = new XeroIntegrationService(settings);
            Commands = new List<IInvoiceCommand>
            {
                new InvoiceCommand("Email", "Email", ExecuteEmailInvoiceCommand),
                new InvoiceCommand("Approve", "AccountCheck", ExecuteApproveInvoiceCommand),
                new InvoiceCommand("Save", "ContentSave", ExecuteSaveInvoiceCommand)
            };
            tasks = tasks ?? new List<ProjectTask>();
            Projects = new ObservableCollection<ProjectExportViewModel>(tasks.GroupBy(c => c.Project)
                .Select(c => new ProjectExportViewModel
                {
                    Project = c.Key,
                    TaskDefinitions = c.GroupBy(d => d.TaskDefinition)
                        .Select(d => new TaskDefinitionExportViewModel(d.Key)
                        {
                            Tasks = d.ToList()
                        })
                        .ToList()
                }));
            ExportToXeroCommand = new Command(ExecuteExportToXeroCommand);
        }

        private void ExecuteSaveInvoiceCommand(object obj)
        {
            _integrationService.SaveInvoice((Invoice)obj);
        }

        private void ExecuteApproveInvoiceCommand(object obj)
        {
            _integrationService.ApproveInvoice((Invoice)obj);
        }

        private void ExecuteEmailInvoiceCommand(object obj)
        {
            _integrationService.EmailInvoice((Invoice)obj);
        }

        private async void ExecuteExportToXeroCommand(object obj)
        {
            var view = new InvoiceNumberDialog(_integrationService.GetInvoiceNumber());
            bool result = (bool)await DialogHost.Show(view, "RootDialog");
            if (result)
            {
                List<InvoiceLineItem> lineItems = new List<InvoiceLineItem>();
                foreach (var project in Projects)
                {
                    foreach (var task in project.TaskDefinitions.GroupBy(c => c.TaskDefinition))
                    {
                        lineItems.Add(new InvoiceLineItem {Description = $"{project.Project.Name}: {task.Key.Name}", Quantity = task.Sum(c => c.Quantity), UnitAmount = task.Key.Rate});
                    }
                }
                var maxDate = Projects.SelectMany(c => c.TaskDefinitions).SelectMany(c => c.Tasks).Max(c => c.Date);
                _projectRepo.Save();
                var invoiceResult = _integrationService.CreateInvoice(lineItems, view.InvoiceTextBox.Text, new DateTime(maxDate.Year, maxDate.Month, DateTime.DaysInMonth(maxDate.Year, maxDate.Month)));
                if (!invoiceResult.IsScuccess)
                {
                    var messageDialog = new MessageDialog($"Validation Errors! {Environment.NewLine}{string.Join(Environment.NewLine, invoiceResult.Errors)}");
                    await DialogHost.Show(messageDialog, "RootDialog");
                }
                else
                {
                    CurrentInvoice = invoiceResult.Invoice;
                    _integrationService.DownloadInvoice(CurrentInvoice);
                }
            }
        }

        private void DownloadInvoice(XeroInvoice invoice)
        {
            //TODO:DF PORT
            //NO idea what the deal is here, props missing, might be really old...
            //ShowInvoice(this, new EventArgs<Invoice>(null));
            //_xeroService.DownloadInvoice(invoice.Id, $"{invoice.Number}.pdf");
            //ShowInvoice(this, new EventArgs<Invoice>(invoice));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
