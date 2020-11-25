using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using AbakHelper.Models;
using AbakHelper.Services;
using AbakHelper.ViewModels;
using Awesomium.Core;
using Awesomium.Windows.Controls;
using Newtonsoft.Json;
using Xero.Api.Infrastructure.ThirdParty.HttpUtility;

namespace AbakHelper
{
    public partial class MainWindow
    {
        private readonly ProjectRepository _projectRepo;
        public ObservableCollection<Project> Projects { get; }
        
        private List<ProjectTask> _currentlyTrackedProjectTasks;
        private readonly SettingsRepository _settingsRepo;

        public MainWindow()
        {
            InitializeComponent();

            _projectRepo = new ProjectRepository();
            _settingsRepo = new SettingsRepository();
            Projects = new ObservableCollection<Project>(_projectRepo.All());
            WebCore.Initialized += WebCore_Initialized;
            InitializeComponent();
            webBrowser.Source = new Uri("https://payme.objectsharp.com");
        }

        private void WebCore_Initialized(object sender, CoreStartEventArgs e)
        {
            var resourceInterceptor = new Interceptor(webBrowser, _projectRepo);
            resourceInterceptor.ProjectsCatalogued += OnProjectsCatalogued;
            WebCore.ResourceInterceptor = resourceInterceptor;
        }

        private void OnProjectsCatalogued(object sender, EventArgs<List<ProjectTask>> e)
        {
            _currentlyTrackedProjectTasks = e.Data;
            MainSnackbar.MessageQueue.Enqueue($"Tracked {e.Data.Select(c=> c.Project.Name).Distinct().Count()} projects and {e.Data.Count} items.");
        }        
        
        private void ExportEntriesButton_Click(object sender, RoutedEventArgs e)
        {
            webBrowser.Visibility = Visibility.Collapsed;
            exportView.SetDataContext(new ExportViewModel(_currentlyTrackedProjectTasks, _projectRepo, _settingsRepo));
            exportView.Visibility = Visibility.Visible;
            optionsView.Visibility = Visibility.Collapsed;
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            webBrowser.Visibility = Visibility.Collapsed;
            exportView.Visibility = Visibility.Collapsed;
            optionsView.DataContext = new OptionsViewModel(_settingsRepo, _projectRepo);
            optionsView.Visibility = Visibility.Visible;
        }

        private void OnAbakButtonClick(object sender, RoutedEventArgs e)
        {
            webBrowser.Visibility = Visibility.Visible;
            exportView.Visibility = Visibility.Collapsed;
            optionsView.Visibility = Visibility.Collapsed;
        }
    }

    public class Interceptor : IResourceInterceptor
    {
        private readonly WebControl _webBrowser;
        private readonly ProjectRepository _projectRepo;
        public event EventHandler<EventArgs<List<ProjectTask>>> ProjectsCatalogued = (s,e) => { };
        public Interceptor(WebControl webBrowser, ProjectRepository projectRepo)
        {
            _webBrowser = webBrowser;
            _projectRepo = projectRepo;
        }

        public ResourceResponse OnRequest(ResourceRequest request)
        {
            if (request.Url.ToString().EndsWith("/GetGroupedTransacts"))          
                RequestData(request[0].Bytes);
            return null;
        }

        public bool OnFilterNavigation(NavigationRequest request)
        {
            return false;
        }

        private void RequestData(string queryString)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                using (JSObject myGlobalObject = _webBrowser.CreateGlobalJavascriptObject("myGlobalObject"))
                {
                    // The handler is of type JavascriptMethodEventHandler. Here we define it
                    // using a lambda expression.
                    myGlobalObject.Bind("AjaxComplete", (s, ev) =>
                    {
                        //Json is in here: ev.Arguments[0];
                        var json = ev.Arguments[0].ToString();
                        json = Regex.Replace(json, "(new\\ Date\\(\\d{4},\\d{2},\\d{2},00,00,00\\))", "\"$1\"");
                        var data = JsonConvert.DeserializeObject<AbakPayload>(json);
                        /* SAMPLE
                         {
	                        "data" : [{
			                        "Descriptions" : null,
			                        "TransactType" : "T",
			                        "CodeDep" : "",
			                        "DisplayDesc" : "Consulting by Associate (CONS)",
			                        "DisplayCode" : "CONS",
			                        "Quantity" : 1.0000,
			                        "HasNote" : false,
			                        "Id" : "2018060709300160",
			                        "Date" : "new Date(2018,05,04,00,00,00)",
			                        "Description" : "SSL troubleshooting",
			                        "PayCode" : "REG",
			                        "PayCodeDescription" : "Regular (REG)",
			                        "IsBillable" : true,
			                        "IsRefundable" : false,
			                        "ClientName" : "Versent Corporation ULC (dba. Laser Quest) (VERSENTLASER)",
			                        "ProjectName" : "Versent LaserQuest - Smart Entertainment Book My Party Upgra (CONS0000591)",
			                        "PhaseName" : "",
			                        "TaskDesc" : "Consulting by Associate (CONS)",
			                        "PhaseId" : "",
			                        "TimeStart" : null,
			                        "TimeEnd" : null,
			                        "IsAffectingTimebank" : true,
			                        "IsTransactApproved" : true,
			                        "IsTransactProjectApproved" : false,
			                        "IsInvoiced" : false,
			                        "InvoicingStatus" : "T",
			                        "IsTransferredToPayroll" : false,
			                        "IsAccepted" : false,
			                        "IsRefused" : false,
			                        "IsWaiting" : false,
			                        "Reference" : "",
			                        "TimesheetQuantity" : 0.0,
			                        "APTransferStatus" : "",
			                        "IsHourly" : false,
			                        "PhaseParentId" : "",
			                        "PhaseParentName" : "",
			                        "PhaseRootId" : "",
			                        "PhaseRootName" : "",
			                        "HasDocuments" : false,
			                        "IsProjectApproved" : false,
			                        "ProjectApprover" : "",
			                        "IsTransactLocked" : false,
			                        "IsDeletable" : false
		                        }]
                                }
                         
                         */
                        List<ProjectTask> tracked = new List<ProjectTask>();
                        foreach (var record in data.Data)
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
                                taskDefinition = new ProjectTaskDefinition {Name = record.DisplayDesc, Rate = project.Rate};
                                project.TaskDefinitions.Add(taskDefinition);
                                _projectRepo.Save();
                            }

                            if (taskDefinition.Rate == 0)
                                taskDefinition.Rate = project.Rate;
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
                        ProjectsCatalogued(this, new EventArgs<List<ProjectTask>>(tracked));
                        return null;
                    });
                }
                try
                {
                    var dict = HttpUtility.ParseQueryString(queryString);
                    var json = JsonConvert.SerializeObject(dict.AllKeys.ToDictionary(k => k, k => dict[k]));
                    //var data = "{groupBy:'TransactType', groupDir: 'ASC', summaryFields: 'Quantity', summaryTypes: 'sum', sort: 'Date', dir: 'ASC', employe: '2003113023121162', date: '2016-10-30T00:00:00', range: 'Weekly' }";
                    string extScript = $"Ext.Ajax.request({{url: \"https://payme.objectsharp.com/Abak/Transact/GetGroupedTransacts?ignore\",params: {json},dataType: \"json\",type: \"POST\",success: function(result) {{myGlobalObject.AjaxComplete(result.responseText);}}}});";
                    Debug.WriteLine(extScript);
                    _webBrowser.ExecuteJavascript(extScript);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }));
        }

    }

    public class EventArgs<T> : EventArgs
    {
        public T Data { get; set; }

        public EventArgs(T data)
        {
            Data = data;
        }
    }
}
