using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using AbakHelper.Integration.Models;
using Awesomium.Core;
using Awesomium.Windows.Controls;
using Microsoft.QueryStringDotNET;
using Newtonsoft.Json;

namespace AbakHelperV2
{
    public class Interceptor : IResourceInterceptor
    {
        private readonly WebControl _webBrowser;
        public event EventHandler<EventArgs<List<TimeTrackerItem>>> TimeEntriesLoaded = (s,e) => { };
        public Interceptor(WebControl webBrowser)
        {
            _webBrowser = webBrowser;
        }

        public ResourceResponse OnRequest(ResourceRequest request)
        {
            var url = request.Url.ToString();
            if (url.EndsWith("/GetGroupedTransacts"))          
                RequestData(request[0].Bytes, url);
            return null;
        }

        public bool OnFilterNavigation(NavigationRequest request)
        {
            return false;
        }

        private void RequestData(string queryString, string url)
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
			                        "ClientName" : "client name removed by Dan",
			                        "ProjectName" : "Project name removed by dan (CONS0000591)",
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
                        TimeEntriesLoaded(this, new EventArgs<List<TimeTrackerItem>>(data.Data));
                        return null;
                    });
                }
                try
                {
                    var dict = QueryString.Parse(queryString).ToDictionary(c=> c.Name, c=> c.Value);
                    var json = JsonConvert.SerializeObject(dict);
                    //var data = "{groupBy:'TransactType', groupDir: 'ASC', summaryFields: 'Quantity', summaryTypes: 'sum', sort: 'Date', dir: 'ASC', employe: '2003113023121162', date: '2016-10-30T00:00:00', range: 'Weekly' }";
                    //the ?df_ignore=1 is so that we don't trap our own call to the server
                    string extScript = $"Ext.Ajax.request({{url: \"{url}?df_ignore=1\",params: {json},dataType: \"json\",type: \"POST\",success: function(result) {{myGlobalObject.AjaxComplete(result.responseText);}}}});";
                    _webBrowser.ExecuteJavascript(extScript);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }));
        }

    }
}