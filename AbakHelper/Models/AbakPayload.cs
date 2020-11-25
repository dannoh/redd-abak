using System;
using System.Collections.Generic;
using AbakHelper.Infrastructure;
using Newtonsoft.Json;

namespace AbakHelper.Models
{
    public class AbakPayload
    {
        [JsonProperty("data")]
        public List<TimeTrackerItem> Data { get; set; }
    }

    public class TimeTrackerItem
    {

        [JsonProperty("Descriptions")]
        public object Descriptions { get; set; }

        [JsonProperty("TransactType")]
        public string TransactType { get; set; }

        [JsonProperty("CodeDep")]
        public string CodeDep { get; set; }

        [JsonProperty("DisplayDesc")]
        public string DisplayDesc { get; set; }

        [JsonProperty("DisplayCode")]
        public string DisplayCode { get; set; }

        [JsonProperty("Quantity")]
        public double Quantity { get; set; }

        [JsonProperty("HasNote")]
        public bool HasNote { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Date")]
        [JsonConverter(typeof(AbakJsonDateTimeConverter))]
        public DateTime Date { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("PayCode")]
        public string PayCode { get; set; }

        [JsonProperty("PayCodeDescription")]
        public string PayCodeDescription { get; set; }

        [JsonProperty("IsBillable")]
        public bool IsBillable { get; set; }

        [JsonProperty("IsRefundable")]
        public bool IsRefundable { get; set; }

        [JsonProperty("ClientName")]
        public string ClientName { get; set; }

        [JsonProperty("ProjectName")]
        public string ProjectName { get; set; }

        [JsonProperty("PhaseName")]
        public string PhaseName { get; set; }

        [JsonProperty("TaskDesc")]
        public string TaskDesc { get; set; }

        [JsonProperty("PhaseId")]
        public string PhaseId { get; set; }

        [JsonProperty("TimeStart")]
        public object TimeStart { get; set; }

        [JsonProperty("TimeEnd")]
        public object TimeEnd { get; set; }

        [JsonProperty("IsAffectingTimebank")]
        public bool IsAffectingTimebank { get; set; }

        [JsonProperty("IsTransactApproved")]
        public bool IsTransactApproved { get; set; }

        [JsonProperty("IsTransactProjectApproved")]
        public bool IsTransactProjectApproved { get; set; }

        [JsonProperty("IsInvoiced")]
        public bool IsInvoiced { get; set; }

        [JsonProperty("InvoicingStatus")]
        public string InvoicingStatus { get; set; }

        [JsonProperty("IsTransferredToPayroll")]
        public bool IsTransferredToPayroll { get; set; }

        [JsonProperty("IsAccepted")]
        public bool IsAccepted { get; set; }

        [JsonProperty("IsRefused")]
        public bool IsRefused { get; set; }

        [JsonProperty("IsWaiting")]
        public bool IsWaiting { get; set; }

        [JsonProperty("Reference")]
        public string Reference { get; set; }

        [JsonProperty("TimesheetQuantity")]
        public double TimesheetQuantity { get; set; }

        [JsonProperty("APTransferStatus")]
        public string APTransferStatus { get; set; }

        [JsonProperty("IsHourly")]
        public bool IsHourly { get; set; }

        [JsonProperty("PhaseParentId")]
        public string PhaseParentId { get; set; }

        [JsonProperty("PhaseParentName")]
        public string PhaseParentName { get; set; }

        [JsonProperty("PhaseRootId")]
        public string PhaseRootId { get; set; }

        [JsonProperty("PhaseRootName")]
        public string PhaseRootName { get; set; }

        [JsonProperty("HasDocuments")]
        public bool HasDocuments { get; set; }

        [JsonProperty("IsProjectApproved")]
        public bool IsProjectApproved { get; set; }

        [JsonProperty("ProjectApprover")]
        public string ProjectApprover { get; set; }

        [JsonProperty("IsTransactLocked")]
        public bool IsTransactLocked { get; set; }

        [JsonProperty("IsDeletable")]
        public bool IsDeletable { get; set; }
    }
}
