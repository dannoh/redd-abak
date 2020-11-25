using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using AbakHelper.Infrastructure;
using AbakHelper.Models;
using AbakHelper.Services;
using AbakHelper.UserControls;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Xero.Api.Core.Model;

namespace AbakHelper.ViewModels
{
    public class ExportViewModel : ViewModelBase
    {
        public event EventHandler<EventArgs<Invoice>> ShowInvoice = (s, e) => { };
        private readonly XeroIntegrationService _xeroService;

        private readonly ProjectRepository _projectRepo;
        private readonly SettingsRepository _settingsRepo;
        public ObservableCollection<ProjectExportViewModel> Projects { get; }

        public Command ExportToXeroCommand { get; }
        public Command SaveInvoiceCommand { get; }
        public Command ApproveInvoiceCommand { get; }
        public Command EmailInvoiceCommand { get; }

        private Invoice _currentInvoice;
        public Invoice CurrentInvoice
        {
            get { return _currentInvoice; }
            private set
            {
                if (Equals(value, _currentInvoice)) return;
                _currentInvoice = value;
                NotifyPropertyChanged();
            }
        }

        public ExportViewModel(List<ProjectTask> tasks, ProjectRepository projectRepo, SettingsRepository settingsRepo)
        {
            var settings = settingsRepo.Get();
            _xeroService = new XeroIntegrationService(settings.XeroSettings.CertificatePath, settings.XeroSettings.CertificatePassword, settings.XeroSettings.ConsumerKey, settings.XeroSettings.ConsumerSecret, settings.XeroSettings.ContactName);

            _projectRepo = projectRepo;
            _settingsRepo = settingsRepo;
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
            SaveInvoiceCommand = new Command(ExecuteSaveInvoiceCommand);
            ApproveInvoiceCommand = new Command(ExecuteApproveInvoiceCommand);
            EmailInvoiceCommand = new Command(ExecuteEmailInvoiceCommand);
        }

        private void ExecuteEmailInvoiceCommand(object obj)
        {
            var invoice = CurrentInvoice;
            Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
            Microsoft.Office.Interop.Outlook.MailItem mailItem = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
            mailItem.Subject = $"{_settingsRepo.Get().CompanyName} Invoice";
            mailItem.To = invoice.Contact.EmailAddress;

            mailItem.Attachments.Add($"{AppDomain.CurrentDomain.BaseDirectory}{invoice.Number}.pdf");
            mailItem.Display(false);
        }

        private void ExecuteApproveInvoiceCommand(object obj)
        {
            CurrentInvoice.Status = Xero.Api.Core.Model.Status.InvoiceStatus.Authorised;
            CurrentInvoice = _xeroService.UpdateInvoice(CurrentInvoice);
           // DownloadInvoice(CurrentInvoice);
        }

        private void ExecuteSaveInvoiceCommand(object obj)
        {            
            SaveFileDialog dialog = new SaveFileDialog { AddExtension = true, DefaultExt = ".pdf", FileName = $"{CurrentInvoice.Number}.pdf"};
            if (dialog.ShowDialog() ?? false)
                System.IO.File.Copy($"{AppDomain.CurrentDomain.BaseDirectory}{CurrentInvoice.Number}.pdf", dialog.FileName);
        }

        private async void ExecuteExportToXeroCommand(object obj)
        {           
            var view = new InvoiceNumberDialog(_xeroService.GetMostRecentInvoiceNumber());
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
                var invoiceResult = _xeroService.CreateInvoice(lineItems, view.InvoiceTextBox.Text, new DateTime(maxDate.Year, maxDate.Month, DateTime.DaysInMonth(maxDate.Year, maxDate.Month)));
                if (!invoiceResult.IsScuccess)
                {
                    var messageDialog = new MessageDialog($"Validation Errors! {Environment.NewLine}{string.Join(Environment.NewLine, invoiceResult.Errors)}");
                    await DialogHost.Show(messageDialog, "RootDialog");
                }
                else
                {
                    CurrentInvoice = invoiceResult.Invoice;
                    DownloadInvoice(invoiceResult.Invoice);
                }
            }
        }

        private void DownloadInvoice(Invoice invoice)
        {
            ShowInvoice(this, new EventArgs<Invoice>(null));
            _xeroService.DownloadInvoice(invoice.Id, $"{invoice.Number}.pdf");            
            ShowInvoice(this, new EventArgs<Invoice>(invoice));
        }
    }

    public class ProjectExportViewModel
    {
        public Project Project { get; set; }

        public List<TaskDefinitionExportViewModel> TaskDefinitions { get; set; }
    }

    public class TaskDefinitionExportViewModel : ViewModelBase
    {
        public ProjectTaskDefinition TaskDefinition { get; }
        public decimal Quantity {  get { return (decimal)Tasks.Sum(c => c.Quantity); } }
        public decimal Amount {  get { return Quantity * TaskDefinition.Rate; } }
        
        public List<ProjectTask> Tasks { get; set; }

        public TaskDefinitionExportViewModel(ProjectTaskDefinition definition)
        {
            TaskDefinition = definition;
            TaskDefinition.PropertyChanged += OnTaskDefinitionPropertyChanged;
        }

        private void OnTaskDefinitionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TaskDefinition.Rate))
                NotifyPropertyChanged(nameof(Amount));
        }

    }
}
